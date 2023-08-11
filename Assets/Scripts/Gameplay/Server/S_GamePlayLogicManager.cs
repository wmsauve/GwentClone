using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StringContainer : INetworkSerializable
{
    public string Text;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            serializer.GetFastBufferWriter().WriteValueSafe(Text);
        }
        else
        {
            serializer.GetFastBufferReader().ReadValueSafe(out Text);
        }
    }
}

public class S_GamePlayLogicManager : NetworkBehaviour
{
    private struct CardNames
    {
        public string[] _cards;

        public CardNames(string[] cards)
        {
            _cards = cards;
        }
    }

    public struct PlayerScores
    {
        private ulong _id;
        public ulong ID { get { return _id; } }

        private int _frontScore;
        public int FrontScores { get { return _frontScore; } }
        private int _rangedScore;
        public int RangedScore { get { return _rangedScore; } }
        private int _siegeScore;
        public int SiegeScore { get { return _siegeScore; } }

        public PlayerScores(ulong id)
        {
            _id = id;
            _frontScore = 0;
            _rangedScore = 0;
            _siegeScore = 0;
        }

        public void IncrementScore(EnumUnitPlacement _cardPlace, int _power)
        {
            switch (_cardPlace)
            {
                case EnumUnitPlacement.Frontline: _frontScore += _power; break;
                case EnumUnitPlacement.Ranged: _rangedScore += _power; break;
                case EnumUnitPlacement.Siege: _siegeScore += _power; break;
                default:
                    GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Chose placement not conducive to setting scores.");
                    break;
            }
        }

        public int TotalScore()
        {
            return _frontScore + _rangedScore + _siegeScore;
        }

        public void ResetScores()
        {
            _frontScore = 0;
            _rangedScore = 0;
            _siegeScore = 0;
        }
    }

    public struct MatchScores
    {
        public PlayerScores[] _players;
        
        public MatchScores(ulong[] _ids)
        {
            _players = new PlayerScores[_ids.Length];
            for(int i = 0; i < _ids.Length; i++)
            {
                _players[i] = new PlayerScores(_ids[i]);
            }
        }

        [System.Serializable]
        public struct ScoresToClient
        {
            public ulong _id;
            public int _front;
            public int _ranged;
            public int _siege;
        }

        public void IncrementScore(EnumUnitPlacement _cardPlace, int _cardPower, ulong _playerId)
        {
            var _int = GeneralPurposeFunctions.FindIndexByPropertyValue(_players, player => player.ID == _playerId);
            if (_int == -1)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "You searched for player ID that doesn't exist.");
                return;
            }

            _players[_int].IncrementScore(_cardPlace, _cardPower);
        }

        public string PassScoresToClient()
        {
            int _length = _players.Length;
            ScoresToClient[] _toClient = new ScoresToClient[_length];
            for(int i = 0; i < _length; i++)
            {
                ScoresToClient _score = new ScoresToClient();
                _score._id = _players[i].ID;
                _score._front = _players[i].FrontScores;
                _score._ranged = _players[i].RangedScore;
                _score._siege = _players[i].SiegeScore;
                _toClient[i] = _score;
            }

            return GeneralPurposeFunctions.ConvertArrayToJson(_toClient);
        }
        
        public void ResetScoresToZero()
        {
            for(int i = 0; i < _players.Length; i++)
            {
                _players[i].ResetScores();
            }
        }

        public ulong GetWinnningPlayerID()
        {
            ulong currentWinner = 0;
            int currentHighest = 0;
            bool drawnGame = false;
            for(int i = 0; i < _players.Length; i++)
            {
                int scoreCheck = _players[i].TotalScore();
                if (scoreCheck == currentHighest)
                {
                    drawnGame = true;
                    continue;
                }

                if(scoreCheck > currentHighest)
                {
                    currentHighest = scoreCheck;
                    currentWinner = _players[i].ID;
                    if (drawnGame) drawnGame = false;
                }
            }

            if (drawnGame) return GlobalConstantValues.GAME_DRAWNGAME;

            return currentWinner;

        }
    }

    private List<C_PlayerGamePlayLogic> _playersLogic = new List<C_PlayerGamePlayLogic>();
    private UI_MulliganCards _mulliganScreen = null;
    private C_PlayerCardsUIManager _cardsInHandScreen = null;
    private GameplayUICanvas _canvasUI = null;
    private C_ZonesManager _zoneManager = null;
    private C_GraveyardManager _graveYardManager = null;
    private S_DeckManagers _deckManager = null;
    private S_TurnManager _turnManager = null;
    private S_SpellsManager _spellsManager = null;

    private int _currentActive;
    private EnumGameplayPhases _currentPhase = EnumGameplayPhases.CoinFlip;
    private MatchScores _currentMatchScores;
    private C_PlayerGamePlayLogic _winningPlayer = null;

    private void OnEnable()
    {
        GlobalActions.OnGameStart += InitializePlayerLogic;
        GlobalActions.OnPhaseChange += NewPhase;

    }

    private void OnDisable()
    {
        GlobalActions.OnGameStart -= InitializePlayerLogic;
        GlobalActions.OnPhaseChange -= NewPhase;
    }

    private void InitializePlayerLogic()
    {
        var _getLogic = FindObjectsOfType<C_PlayerGamePlayLogic>();
        if(_getLogic == null || _getLogic.Length == 0)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "There is no player logic in the scene.");
            return;
        }

        _deckManager = GetComponent<S_DeckManagers>();
        _turnManager = GetComponent<S_TurnManager>();
        _spellsManager = GetComponent<S_SpellsManager>();
        _spellsManager._gameManager = this;

        if (_deckManager == null || _turnManager == null || _spellsManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Your GameLogic manager and Deck/Turn manager should be on the same gameobject.");
            return;
        }

        //Initialize player logic.
        _playersLogic = _getLogic.ToList();
        for (int i = 0; i < _playersLogic.Count; i++)
        {
            var id = _playersLogic[i].gameObject.GetComponent<NetworkObject>().OwnerClientId;
            foreach(GwentPlayer player in _deckManager.GwentPlayers)
            {
                if(id == player.ID)
                {
                    _playersLogic[i].InitializePlayerLogic(player);
                    continue;
                }
            }
        }

        //pick first player.
        _currentActive = Random.Range(0, _playersLogic.Count);
        SetPlayerTurnActive();

        //
        CreateMatchScores();
    }

    private void NewPhase(EnumGameplayPhases _phase)
    {
        switch (_phase) 
        {
            case EnumGameplayPhases.Mulligan:
                if (IsServer)
                {
                    foreach(C_PlayerGamePlayLogic player in _playersLogic)
                    {
                        string[] cardNames = player.CreateInitialHand();
                        var _json = JsonUtility.ToJson(new CardNames(cardNames));
                        ShowMulliganScreenClientRpc(_json, player.ClientRpcParams);
                    }

                    _currentPhase = _phase;
                }

                break;
            case EnumGameplayPhases.Regular:

                if (IsServer)
                {
                    if(_currentPhase == EnumGameplayPhases.Regular)
                    {
                        _currentActive++;
                        if (_currentActive == _playersLogic.Count) _currentActive = 0;
                        SetPlayerTurnActive();
                    }

                    if(_currentPhase == EnumGameplayPhases.MatchOver)
                    {
                        _currentActive++;
                        if (_currentActive == _playersLogic.Count) _currentActive = 0;
                        SetPlayerTurnActive();

                        _currentMatchScores.ResetScoresToZero();
                    }

                    foreach (C_PlayerGamePlayLogic player in _playersLogic)
                    {
                        if (_currentPhase == EnumGameplayPhases.Mulligan)
                        {
                            player.EndMulliganPhase();
                            string[] cardNames = player.ReturnCardIds(EnumCardListType.Hand);
                            var _json = JsonUtility.ToJson(new CardNames(cardNames));
                            StartFirstRegularTurnClientRpc(_json, player.TurnActive, player.ClientRpcParams);
                        }
                        else
                        {
                            PassTurnSwapClientRpc(player.TurnActive, player.ClientRpcParams);
                        }
                    }

                    _currentPhase = _phase;
                }

                break;
            case EnumGameplayPhases.MatchOver:
                if (IsServer)
                {
                    ulong _winner = _currentMatchScores.GetWinnningPlayerID();
                    C_PlayerGamePlayLogic _loser = null;

                    if(_winner == GlobalConstantValues.GAME_DRAWNGAME)
                    {
                        foreach (C_PlayerGamePlayLogic player in _playersLogic)
                        {
                            player.DecrementLives();
                            if (player.Lives == 0) _loser = player;
                        }
                    }
                    else
                    {
                        foreach (C_PlayerGamePlayLogic player in _playersLogic)
                        {
                            string name = player.MyInfo.Username;

                            if (player.MyInfo.ID != _winner)
                            {
                                player.DecrementLives();
                                if (player.Lives == 0) _loser = player;
                            }
                            else
                            {
                                _winningPlayer = player;
                                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, $"Winning player: {name}");
                            }
                        }
                    }

                    for (int i = 0; i < _playersLogic.Count; i++)
                    {
                        var myLives = GlobalConstantValues.GAME_INITIALLIVES;
                        var opponentLives = GlobalConstantValues.GAME_INITIALLIVES;
                        var id = _playersLogic[i].gameObject.GetComponent<NetworkObject>().OwnerClientId;
                        foreach (C_PlayerGamePlayLogic player in _playersLogic)
                        {
                            if(player.MyInfo.ID == id)
                            {
                                myLives = player.Lives;
                            }
                            else
                            {
                                opponentLives = player.Lives;
                            }
                        }
                        _playersLogic[i].EndOfTurnGraveyardCards();
                        string[] cardNames = _playersLogic[i].ReturnCardIds(EnumCardListType.Graveyard);
                        var _json = JsonUtility.ToJson(new CardNames(cardNames));
                        UpdateGraveyardClientRpc(_json, _playersLogic[i].ClientRpcParams);
                        SetHealthCrystalsClientRpc(myLives, opponentLives, _playersLogic[i].ClientRpcParams);
                    }

                    if (_loser != null)
                    {
                        _turnManager.GameStart = false;
                    }
                    else StateMessageInUIClientRpc($"Match {_turnManager.MatchCounter - 1} Ended."); //incremented already.
                    EndOfMatchHandlingClientRpc();
                    _currentPhase = _phase;
                }

                break;
            case EnumGameplayPhases.GameOver:
                ShowEndOfGameScreenClientRpc();
                if(_winningPlayer != null) StateMessageInUIClientRpc($"{_winningPlayer.MyInfo.Username} has won.");
                break;
            default:
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Error with ending phases.");
                break;
        }
    }

    private void SetPlayerTurnActive()
    {
        for(int i = 0; i < _playersLogic.Count; i++)
        {
            if (i == _currentActive)
            {
                _playersLogic[i].TurnActive = true;
                StateMessageInUIClientRpc($"It is {_playersLogic[i].MyInfo.Username}'s turn.");
            }
            else _playersLogic[i].TurnActive = false;
        }
    }

    private void CreateListOfCardsFromString(ref List<Card> _cards, string names)
    {
        CardNames _cardNames = JsonUtility.FromJson<CardNames>(names);
        foreach (string name in _cardNames._cards)
        {
            Card newCard = _deckManager.CardRepo.GetCard(name);
            if (newCard == null)
            {
                Debug.LogWarning(name);
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Invalid card name from server to client.");
                return;
            }

            _cards.Add(newCard);
        }
    }

    private bool ValidateCardFromServer(string cardName, int cardSlot, List<Card> _cards)
    {
        int whichCard = 0;
        foreach (Card card in _cards)
        {
            if (card.id == cardName && cardSlot == whichCard) return true;
            whichCard++;
        }
        return false;
    }

    private void CreateMatchScores()
    {
        int _playerCount = _playersLogic.Count;
        ulong[] _listOfIds = new ulong[_playerCount];
        for(int i = 0; i < _playerCount; i++)
        {
            _listOfIds[i] = _playersLogic[i].MyInfo.ID;
        }

        _currentMatchScores = new MatchScores(_listOfIds);
    }

    #region Client RPC
    [ClientRpc]
    private void ShowEndOfGameScreenClientRpc(ClientRpcParams clientRpcParams = default)
    {

    }

    [ClientRpc]
    public void DestroyCardsFromEffectClientRpc(int _powerToDestroy)
    {
        //Todo: This needs to check for unit placement in the future.
        _zoneManager.DestroyCardsOfPowerDueToScorch(_powerToDestroy);
    }

    [ClientRpc]
    private void UpdateGraveyardClientRpc(string cardNames, ClientRpcParams clientRpcParams = default)
    {
        _graveYardManager = GeneralPurposeFunctions.GetComponentFromGameObject<C_GraveyardManager>(gameObject);
        if (_graveYardManager == null) return;

        List<Card> _cards = new List<Card>();
        CreateListOfCardsFromString(ref _cards, cardNames);
        _graveYardManager.PassCardsToGraveyard(_cards);
    }

    [ClientRpc]
    private void ShowMulliganScreenClientRpc(string cardNames, ClientRpcParams clientRpcParams = default)
    {
        _deckManager = GeneralPurposeFunctions.GetComponentFromGameObject<S_DeckManagers>(gameObject);
        if (_deckManager == null) return;
        
        Debug.LogWarning(cardNames);

        var _mulligan = Resources.FindObjectsOfTypeAll<UI_MulliganCards>();
        if (_mulligan == null || _mulligan.Length > 1)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "You should only have one mulligan object.");
            return;
        }
        if (_mulligan.Length == 0)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You don't have the screen added for Mulligan phase.");
            return;
        }

        _mulliganScreen = _mulligan[0];

        if (_mulliganScreen == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You don't have the screen added for Mulligan phase.");
            return;
        }

        List<Card> _cards = new List<Card>();
        CreateListOfCardsFromString(ref _cards, cardNames);
        _mulliganScreen.InitializeMulliganCards(_cards, this, GlobalConstantValues.GAME_MULLIGANSAMOUNT);
        _mulliganScreen.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void StartFirstRegularTurnClientRpc(string cardNames, bool isActive, ClientRpcParams clientRpcParams = default)
    {
        _mulliganScreen.gameObject.SetActive(false);

        var _cardInHand = Resources.FindObjectsOfTypeAll<C_PlayerCardsUIManager>();
        var _canvasUIs = Resources.FindObjectsOfTypeAll<GameplayUICanvas>();
        if (_cardInHand == null || _cardInHand.Length > 1 || _canvasUIs == null || _canvasUIs.Length > 1)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "You should only have one of these client UI components.");
            return;
        }
        if (_cardInHand.Length == 0 || _canvasUIs.Length == 0)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You don't have the screen added for Card in hand or main UI component phase.");
            return;
        }

        List<Card> _cards = new List<Card>();
        CreateListOfCardsFromString(ref _cards, cardNames);
        _cardsInHandScreen = _cardInHand[0];
        _cardsInHandScreen.InitializeHand(_cards, this);
        _cardsInHandScreen.gameObject.SetActive(true);

        _canvasUI = _canvasUIs[0];
        _canvasUI.SetActivePlayer(isActive);
    }

    [ClientRpc]
    public void MulliganACardClientRpc(string cardName, int mulliganCount, ClientRpcParams clientRpcParams = default)
    {
        var newCard =_deckManager.CardRepo.GetCard(cardName);
        if(newCard == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Doctored client receiving card from server.");
            return;
        }

        _mulliganScreen.UpdateMulliganedButton(newCard, mulliganCount);
    }

    [ClientRpc]
    public void PlacePlayedCardClientRpc(string cardName, EnumUnitPlacement cardPlace)
    {
        if (_zoneManager == null) _zoneManager = GeneralPurposeFunctions.GetComponentFromGameObject<C_ZonesManager>(gameObject);
        if (_zoneManager == null) return;

        var _cardData = _deckManager.CardRepo.GetCard(cardName);
        _zoneManager.AddCardToZone(_cardData, cardPlace);
    }

    [ClientRpc]
    public void EndOfMatchHandlingClientRpc()
    {
        if (_zoneManager == null) _zoneManager = GeneralPurposeFunctions.GetComponentFromGameObject<C_ZonesManager>(gameObject);
        if (_zoneManager == null) return;

        _zoneManager.CleanZones();
        //_cardsInHandScreen.CancelCardSelection();
        GlobalActions.OnNotPlayingHeldCard?.Invoke();
    } 

    [ClientRpc]
    public void FixUIAfterPlayedCardClientRpc(int cardSlot, ClientRpcParams clientRpcParams = default)
    {
        _cardsInHandScreen.RemoveCardFromHand(cardSlot);
    }

    [ClientRpc]
    public void HandleScoresOnUIClientRpc(string _scores)
    {
        GeneralPurposeFunctions.ArrayWrapper<MatchScores.ScoresToClient> _newScores = JsonUtility.FromJson<GeneralPurposeFunctions.ArrayWrapper<MatchScores.ScoresToClient>>(_scores);
        //Add score to client UI
        _canvasUI.SetNewScores(_newScores.array);
    }

    [ClientRpc]
    public void PassTurnSwapClientRpc(bool isActive, ClientRpcParams clientRpcParams = default)
    {
        if (_canvasUI == null || _cardsInHandScreen == null) return;
        _canvasUI.SetActivePlayer(isActive);
        //_cardsInHandScreen.CancelCardSelection();
        GlobalActions.OnNotPlayingHeldCard?.Invoke();
    }

    [ClientRpc]
    public void SetHealthCrystalsClientRpc(int _playerLives, int _opponentLives, ClientRpcParams clientRpcParams = default)
    {
        if (_canvasUI == null) return;
        _canvasUI.SetCrystals(_playerLives, _opponentLives);
    }

    [ClientRpc]
    private void StateMessageInUIClientRpc(string _message)
    {
        GlobalActions.OnDisplayFeedbackInUI?.Invoke(_message);
    }
    #endregion Client RPC

    #region Server RPC
    [ServerRpc(RequireOwnership = false)]
    public void MulliganACardServerRpc(string cardName, int cardSlot, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        var _play = _playersLogic.Find((logic) => logic.MyInfo.ID == clientId);

        if(!ValidateCardFromServer(cardName, cardSlot, _play.CardsInHand))
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.InvalidInput, "Invalid Card sent from client to server.", _play.MyInfo.Username);
            return;
        }

        string _newCard = _play.MulliganCard(cardName);
        if (_newCard == string.Empty) return;
        MulliganACardClientRpc(_newCard, _play.Mulligans, _play.ClientRpcParams);

        //run mulligan end check
        foreach(C_PlayerGamePlayLogic player in _playersLogic)
        {
            if (player.Mulligans > 0) return;
        }

        _turnManager.EndMulliganPhase();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayCardDuringTurnServerRpc(string cardName, int cardSlot, EnumUnitPlacement cardPlace, StringContainer[] _interactCards = null, ServerRpcParams serverRpcParams = default)
    {
        if(_interactCards != null && _interactCards.Length > 0)
        {
            Debug.LogWarning(_interactCards[0].Text + " card yo.");
        }

        if (_turnManager == null || _turnManager.CurrentPhase != EnumGameplayPhases.Regular) return;

        var clientId = serverRpcParams.Receive.SenderClientId;
        var _play = _playersLogic.Find((logic) => logic.MyInfo.ID == clientId);

        if (!_play.TurnActive)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.InvalidInput, "Wrong player sending inputs to server.", _play.MyInfo.Username);
            return;
        }

        if (!ValidateCardFromServer(cardName, cardSlot, _play.CardsInHand))
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.InvalidInput, "Invalid Card sent from client to server.", _play.MyInfo.Username);
            return;
        }

        Card _card = _deckManager.CardRepo.GetCard(cardName);
        bool isUnit = true;
        //Handle Card
        if(_card.unitType == EnumUnitType.Regular || _card.unitType == EnumUnitType.Spy)
        {
            //Handle Spy
            PlacePlayedCardClientRpc(cardName, cardPlace);
        }
        // spells
        else
        {
            isUnit = false;
            _spellsManager.HandleSpell(_card.cardEffects, _playersLogic, clientId);

            foreach(C_PlayerGamePlayLogic _player in _playersLogic)
            {
                string[] cardNames = _player.ReturnCardIds(EnumCardListType.Graveyard);
                var _json = JsonUtility.ToJson(new CardNames(cardNames));
                UpdateGraveyardClientRpc(_json, _player.ClientRpcParams);
            }
        }
        FixUIAfterPlayedCardClientRpc(cardSlot, _play.ClientRpcParams);
        _play.SuccessfullyPlayCards(cardSlot, cardPlace, isUnit);

        //Score
        if (_card.cardPower > 0)
        {
            _currentMatchScores.IncrementScore(cardPlace, _card.cardPower, clientId);
            var _json = _currentMatchScores.PassScoresToClient();
            HandleScoresOnUIClientRpc(_json);
        }

        _turnManager.EndRegularTurn(false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PassYourTurnServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        var _play = _playersLogic.Find((logic) => logic.MyInfo.ID == clientId);

        if (!_play.TurnActive)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.InvalidInput, "Wrong player sending inputs to server.", _play.MyInfo.Username);
            return;
        }

        _turnManager.EndRegularTurn(true);

    }
    #endregion Server RPC
}
