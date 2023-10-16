using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class S_GamePlayLogicManager : NetworkBehaviour
{
    //public struct CardNames
    //{
    //    public string[] _cards;

    //    public CardNames(string[] cards)
    //    {
    //        _cards = cards;
    //    }
    //}

    /// <summary>
    /// Rename this later. Use to pass cards through network to identify cards.
    /// </summary>
    [System.Serializable]
    public struct CardToClient
    {
        public string _card;
        public string _unique;

        public CardToClient(string _name, string _guid)
        {
            _card = _name;
            _unique = _guid;
        }
    }

    public struct CardPlacements
    {
        public int[] _placements;

        public CardPlacements(int[] placements)
        {
            _placements = placements;
        }
    }
    [System.Serializable]
    //public struct InteractTarget
    //{
    //    public string _card;
    //    public int _placement;

    //    public InteractTarget(string name, int placement)
    //    {
    //        _card = name;
    //        _placement = placement;
    //    }
    //}

    //public struct InteractCardsOnServer
    //{
    //    public GwentCard _card;
    //    public int _placement;

    //    public InteractCardsOnServer(GwentCard card, int placement)
    //    {
    //        _card = card;
    //        _placement = placement;
    //    }
    //}


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
                        CardToClient[] cardNames = player.CreateInitialHand();
                        //var _json = JsonUtility.ToJson(new CardNames(cardNames));
                        var _json = GeneralPurposeFunctions.ConvertArrayToJson(cardNames);
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
                            CardToClient[] cardNames = player.ReturnCardIds(EnumCardListType.Hand);
                            //var _json = JsonUtility.ToJson(new CardNames(cardNames));
                            var _json = GeneralPurposeFunctions.ConvertArrayToJson(cardNames);
                            StartFirstRegularTurnClientRpc(_json, player.TurnActive, player.ClientRpcParams);
                        }
                        else
                        {
                            PassTurnSwapClientRpc(player.TurnActive, player.ClientRpcParams);
                            player.RunCheckUnresolvedCards();
                            CardToClient[] cardNames = player.ReturnCardIds(EnumCardListType.Graveyard);
                            //var _json = JsonUtility.ToJson(new CardNames(cardNames));
                            var _json = GeneralPurposeFunctions.ConvertArrayToJson(cardNames);
                            UpdateGraveyardClientRpc(_json, player.ClientRpcParams);
                            CloseGraveyardUIClientRpc(player.ClientRpcParams);
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
                        _playersLogic[i].RunCheckUnresolvedCards();
                        CardToClient[] cardNames = _playersLogic[i].ReturnCardIds(EnumCardListType.Graveyard);
                        //var _json = JsonUtility.ToJson(new CardNames(cardNames));
                        var _json = GeneralPurposeFunctions.ConvertArrayToJson(cardNames);
                        UpdateGraveyardClientRpc(_json, _playersLogic[i].ClientRpcParams);
                        SetHealthCrystalsClientRpc(myLives, opponentLives, _playersLogic[i].ClientRpcParams);
                        CloseGraveyardUIClientRpc(_playersLogic[i].ClientRpcParams);
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

    private void CreateListOfCardsFromString(ref List<GwentCard> _cards, string toClient)
    {
        if (toClient == null) return;
        GeneralPurposeFunctions.ArrayWrapper<CardToClient> _cardNames = JsonUtility.FromJson<GeneralPurposeFunctions.ArrayWrapper<CardToClient>>(toClient);
        CardToClient[] _cardToClient = _cardNames.array;

        //CardToClient _cardNames = JsonUtility.FromJson<CardNames>(names);

        for(int i = 0; i < _cardToClient.Length; i++)
        {
            CardToClient fromServer = _cardToClient[i];
            Card newCard = _deckManager.CardRepo.GetCard(fromServer._card);
            if (newCard == null)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, $"Invalid card name from server to client: {name}");
                return;
            }

            GwentCard _newGwentCard = new GwentCard(fromServer._unique, newCard);
            _cards.Add(_newGwentCard);
        }
    }

    private void CreateListOfPlacementsFromString(ref List<int> _placements, string placement)
    {
        if (placement == null) return;
        CardPlacements _cardPlacements = JsonUtility.FromJson<CardPlacements>(placement);
        foreach (int place in _cardPlacements._placements)
        {
            _placements.Add(place);
        }
    }

    #region Validate Info From Client Related

    private bool ValidateCardFromServer(string cardName, int cardSlot, List<GwentCard> _cards)
    {
        int whichCard = 0;

        foreach (GwentCard card in _cards)
        {
            if (card.id == cardName && cardSlot == whichCard) return true;
            whichCard++;
        }
        return false;
    }

    private bool ValidatePlayedMinionPlacement(Card _card, EnumUnitPlacement _target)
    {
        if (_card.cardType != EnumCardType.Unit) return true;

        if (_card.cardEffects.Contains(EnumCardEffects.Agile))
        {
            switch (_card.unitPlacement)
            {
                case EnumUnitPlacement.Agile_FR:
                    if (_target == EnumUnitPlacement.Siege) return false;
                    else return true;
                case EnumUnitPlacement.Agile_FS:
                    if (_target == EnumUnitPlacement.Ranged) return false;
                    else return true;
                case EnumUnitPlacement.Agile_RS:
                    if (_target == EnumUnitPlacement.Frontline) return false;
                    else return true;
                default:
                    GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, $"Only Agile. You tried this placement for Agile validation: {_target}");
                    return false;
            }
        }

        else
        {
            if (_card.unitPlacement == _target) return true;
        }

        return false;
    }

    #endregion Validate Info From Client Related

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

    /// <summary>
    /// For CardSlot = GlobalConstantValues.LOGIC_NULLINT, minion is not played from hand.
    /// </summary>
    /// <param name="_card"></param>
    /// <param name="clientId"></param>
    /// <param name="cardName"></param>
    /// <param name="cardPlace"></param>
    /// <param name="cardSlot"></param>
    /// <param name="_play"></param>
    /// <param name="_interactCards"></param>
    /// <returns></returns>
    private bool HandleLogicFromPlayedCard(
        GwentCard _card, 
        ulong clientId, 
        string cardName, 
        EnumUnitPlacement cardPlace,
        int cardSlot,
        C_PlayerGamePlayLogic _play,
        string _interactCards = null
        )
    {
        CardToClient[] _arrayOfInteracts = new CardToClient[0];
        if (_interactCards != null)
        {
            GeneralPurposeFunctions.ArrayWrapper<CardToClient> _cardNames = JsonUtility.FromJson<GeneralPurposeFunctions.ArrayWrapper<CardToClient>>(_interactCards);
            _arrayOfInteracts = _cardNames.array;
        }
        
        //Handle Card
        if (_card.unitType == EnumUnitType.Regular || _card.unitType == EnumUnitType.Spy)
        {
            //Decoy
            if (_card.cardType == EnumCardType.Special && _interactCards != null)
            {
                _spellsManager.HandleSpell(_card.cardEffects, _arrayOfInteracts, cardPlace, _play, cardSlot, _card);
                CardToClient _decoy = new CardToClient();
                _decoy._card = cardName;
                _decoy._unique = _card.UniqueGuid;
                var _json = JsonUtility.ToJson(_decoy);

                CardToClient _interact = new CardToClient();
                _interact._card = _arrayOfInteracts[0]._card;
                _interact._unique = _arrayOfInteracts[0]._unique;
                var _json2 = JsonUtility.ToJson(_decoy);

                SwapCardInPlayClientRpc(_json, _arrayOfInteracts[0]._unique, cardPlace);
                SwapCardInHandClientRpc(_json2, cardSlot, _play.ClientRpcParams);
                return true;
            }

            //Cards that don't just drop right away. For example, medic needs to go to graveyard.
            if (_card.cardEffects.Contains(EnumCardEffects.Medic) && _play.CardsInGraveyard.Count > 0)
            {
                _spellsManager.HandleSpell(_card, _playersLogic, clientId);
                _play.StoreReferenceToPlayingMultiStepCard(_card, cardPlace, cardSlot);
                return false;
            }

            CardToClient _cardToPlay = new CardToClient();
            _cardToPlay._card = _card.id;
            _cardToPlay._unique = _card.UniqueGuid;
            var _json3 = JsonUtility.ToJson(_cardToPlay);
            PlacePlayedCardClientRpc(_json3, cardPlace, _card.unitType);

            switch (_card.unitType)
            {
                //Maybe refactor if game every doesn't have 1v1.
                case EnumUnitType.Spy:
                    C_PlayerGamePlayLogic _otherPlayer = _playersLogic.Find(x => x.OwnerClientId != _play.OwnerClientId);
                    _otherPlayer.PlaceCardInPlay(_card, cardPlace);
                    break;
                case EnumUnitType.Regular:
                    _play.PlaceCardInPlay(_card, cardPlace);
                    break;
            }
        }

        if(_card.cardEffects != null && _card.cardEffects.Count > 0) _spellsManager.HandleSpell(_card, _playersLogic, clientId);

        //Early skip depending on card. i.e. when multiple cards need handling
        if (_card.cardEffects.Contains(EnumCardEffects.Medic)) return true;

        //Process if not early skip.
        foreach (C_PlayerGamePlayLogic _player in _playersLogic)
        {
            CardToClient[] cardNames = _player.ReturnCardIds(EnumCardListType.Graveyard);
            //var _json = JsonUtility.ToJson(new CardNames(cardNames));
            var _json = GeneralPurposeFunctions.ConvertArrayToJson(cardNames);
            UpdateGraveyardClientRpc(_json, _player.ClientRpcParams);
        }

        //Remove played card if reaching end.
        GwentCard _cardtoDiscard = _play.CardsInHand.Find(x => x.UniqueGuid == _card.UniqueGuid);
        int _indexToDiscard = _play.CardsInHand.FindIndex(x => x.UniqueGuid == _card.UniqueGuid);
        CardToClient _discard = new CardToClient(_cardtoDiscard.id, _cardtoDiscard.UniqueGuid);
        var _jsonD = JsonUtility.ToJson(_discard);

        if (cardSlot != GlobalConstantValues.LOGIC_NULLINT)
        {
            RemoveCardFromHandClientRpc(_jsonD, _play.ClientRpcParams);
            _play.RemoveCardFromHandServer(_indexToDiscard);
        }

        return true;
    }

    private void UpdateScores()
    {
        _currentMatchScores.ResetScoresToZero();

        foreach (C_PlayerGamePlayLogic _player in _playersLogic)
        {
            S_GameZones _zones = _player.CardsInPlay;
            ulong _whichPlayer = _player.ReturnID();
            foreach(GwentCard card in _zones.CardsInFront.Cards) _currentMatchScores.IncrementScore(EnumUnitPlacement.Frontline, card.cardPower, _whichPlayer);
            foreach (GwentCard card in _zones.CardsInRanged.Cards) _currentMatchScores.IncrementScore(EnumUnitPlacement.Ranged, card.cardPower, _whichPlayer);
            foreach (GwentCard card in _zones.CardsInSiege.Cards) _currentMatchScores.IncrementScore(EnumUnitPlacement.Siege, card.cardPower, _whichPlayer);
        }

        var _json = _currentMatchScores.PassScoresToClient();
        HandleScoresOnUIClientRpc(_json);
    }

    //TODO: refactor to not use loop for applying logic to client.
    private void PlayStoredMultiStepCardsFromPlayer(C_PlayerGamePlayLogic _player)
    {
        List<C_PlayerGamePlayLogic.StoreAdditionalStepCards> _multiStepCards = _player.MultiStepCards;
        foreach(C_PlayerGamePlayLogic.StoreAdditionalStepCards _card in _multiStepCards)
        {
            GwentCard _data = _card.CardData;

            EnumUnitPlacement _placement = _card.CardPlace;
            int _slot = _card.CardSlot;

            PlacePlayedCardClientRpc(_data.id, _placement, _data.unitType);
            switch (_data.unitType)
            {
                //Maybe refactor if game every doesn't have 1v1.
                case EnumUnitType.Spy:
                    C_PlayerGamePlayLogic _otherPlayer = _playersLogic.Find(x => x.OwnerClientId != _player.OwnerClientId);
                    _otherPlayer.PlaceCardInPlay(_data, _placement);
                    break;
                case EnumUnitType.Regular:
                    _player.PlaceCardInPlay(_data, _placement);
                    break;
            }

            //Remove played card if reaching end.
            GwentCard _cardtoDiscard = _player.CardsInHand.Find(x => x.UniqueGuid == _data.UniqueGuid);
            int _indexToDiscard = _player.CardsInHand.FindIndex(x => x.UniqueGuid == _data.UniqueGuid);
            CardToClient _discard = new CardToClient(_cardtoDiscard.id, _cardtoDiscard.UniqueGuid);
            var _jsonD = JsonUtility.ToJson(_discard);

            if (_slot != GlobalConstantValues.LOGIC_NULLINT)
            {
                RemoveCardFromHandClientRpc(_jsonD, _player.ClientRpcParams);
                _player.RemoveCardFromHandServer(_indexToDiscard);
            }
        }
    }

    #region Client RPC
    [ClientRpc]
    private void ShowEndOfGameScreenClientRpc(ClientRpcParams clientRpcParams = default)
    {

    }

    [ClientRpc]
    public void DestroyCardsFromEffectClientRpc(int _powerToDestroy, EnumUnitPlacement _placement, bool targetIsPlayer = false, ClientRpcParams clientRpcParams = default)
    {
        _zoneManager.DestroyCardsOfPowerDueToScorch(_powerToDestroy, _placement, targetIsPlayer);
    }

    [ClientRpc]
    private void UpdateGraveyardClientRpc(string cardToClient, ClientRpcParams clientRpcParams = default)
    {
        var _found = GeneralPurposeFunctions.FetchComponentOnClient(ref _graveYardManager, gameObject);
        if (!_found) return;

        List<GwentCard> _cards = new List<GwentCard>();
        CreateListOfCardsFromString(ref _cards, cardToClient);
        Debug.LogWarning(cardToClient + " what's in graveyard?");
        _graveYardManager.PassCardsToGraveyard(_cards);
    }

    [ClientRpc]
    public void OpenGraveyardUIClientRpc(ClientRpcParams clientRpcParams = default)
    {
        var _found = GeneralPurposeFunctions.FetchComponentOnClient(ref _graveYardManager, gameObject);
        if (!_found) return;

        _graveYardManager.ToggleGraveyardUI();
        _cardsInHandScreen.OnNotDraggingThisCard();
    }

    [ClientRpc]
    public void CloseGraveyardUIClientRpc(ClientRpcParams clientRpcParams = default)
    {
        var _found = GeneralPurposeFunctions.FetchComponentOnClient(ref _graveYardManager, gameObject);
        if (!_found) return;

        _graveYardManager.TurnOffGraveyardFromServer();
    }

    [ClientRpc]
    private void ShowMulliganScreenClientRpc(string cardToClient, ClientRpcParams clientRpcParams = default)
    {
        var _found = GeneralPurposeFunctions.FetchComponentOnClient(ref _deckManager, gameObject);
        if (!_found) return;

        Debug.LogWarning(cardToClient);

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

        List<GwentCard> _cards = new List<GwentCard>();
        CreateListOfCardsFromString(ref _cards, cardToClient);
        _mulliganScreen.InitializeMulliganCards(_cards, GlobalConstantValues.GAME_MULLIGANSAMOUNT);
        _mulliganScreen.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void StartFirstRegularTurnClientRpc(string cardToClient, bool isActive, ClientRpcParams clientRpcParams = default)
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

        List<GwentCard> _cards = new List<GwentCard>();
        CreateListOfCardsFromString(ref _cards, cardToClient);
        _cardsInHandScreen = _cardInHand[0];
        _cardsInHandScreen.InitializeHand(_cards, this);
        _cardsInHandScreen.gameObject.SetActive(true);

        _canvasUI = _canvasUIs[0];
        _canvasUI.SetActivePlayer(isActive);
    }

    [ClientRpc]
    public void PlaceCardInHandClientRpc(string cardToClient, ClientRpcParams clientRpcParams = default)
    {
        if (_cardsInHandScreen == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Somehow your cards in hand component on UI is null.");
            return;
        }

        List<GwentCard> _cards = new List<GwentCard>();
        CreateListOfCardsFromString(ref _cards, cardToClient);
        _cardsInHandScreen.DrawCards(_cards);
    }

    [ClientRpc]
    public void MulliganACardClientRpc(string cardName, int mulliganCount, ClientRpcParams clientRpcParams = default)
    {
        var newCard = _deckManager.CardRepo.GetCard(cardName);
        if(newCard == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Doctored client receiving card from server.");
            return;
        }

        GwentCard gwentCard = new GwentCard(newCard);

        _mulliganScreen.UpdateMulliganedButton(gwentCard, mulliganCount);
    }

    [ClientRpc]
    public void PlacePlayedCardClientRpc(string cardToClient, EnumUnitPlacement cardPlace, EnumUnitType cardType)
    {
        if (_zoneManager == null) _zoneManager = GeneralPurposeFunctions.GetComponentFromGameObject<C_ZonesManager>(gameObject);
        if (_zoneManager == null) return;

        CardToClient _card = JsonUtility.FromJson<CardToClient>(cardToClient);
        Card _cardData = _deckManager.CardRepo.GetCard(_card._card);
        GwentCard _gwentCard = new GwentCard(_card._unique, _cardData);

        _zoneManager.AddCardToZone(_gwentCard, cardPlace, cardType);
    }

    [ClientRpc]
    public void PlayMultipleCardsClientRpc(string cardToClient, ClientRpcParams clientRpcParams = default)
    {
        if (_zoneManager == null) _zoneManager = GeneralPurposeFunctions.GetComponentFromGameObject<C_ZonesManager>(gameObject);
        if (_zoneManager == null) return;

        List<GwentCard> _cards = new List<GwentCard>();
        CreateListOfCardsFromString(ref _cards, cardToClient);

        foreach (GwentCard _card in _cards)
        {
            _zoneManager.AddCardToZone(_card, _card.unitPlacement, _card.unitType);
        }

        var _playerOfCard = GeneralPurposeFunctions.GetPlayerLogicReference();
        if (!_playerOfCard.TurnActive) return;

        _cardsInHandScreen.RemoveManyCardsFromHand(_cards);
    }

    [ClientRpc]
    private void SwapCardInPlayClientRpc(string cardPlayed, string targetCard, EnumUnitPlacement cardPlace)
    {
        if (_zoneManager == null) _zoneManager = GeneralPurposeFunctions.GetComponentFromGameObject<C_ZonesManager>(gameObject);
        if (_zoneManager == null) return;

        CardToClient _decoy = JsonUtility.FromJson<CardToClient>(cardPlayed);
        Card _cardData = _deckManager.CardRepo.GetCard(_decoy._card);
        GwentCard _gwentCard = new GwentCard(_decoy._unique, _cardData);
        _zoneManager.SwapCardInZone(_gwentCard, cardPlace, targetCard);
    }

    [ClientRpc]
    private void SwapCardInHandClientRpc(string targetCard, int cardSlot, ClientRpcParams clientRpcParams = default)
    {
        CardToClient _targetCard = JsonUtility.FromJson<CardToClient>(targetCard);
        Card _cardData = _deckManager.CardRepo.GetCard(_targetCard._card);
        GwentCard _gwentCard = new GwentCard(_targetCard._unique, _cardData);

        _cardsInHandScreen.SwapCardInHand(cardSlot, _gwentCard);
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
    public void RemoveCardFromHandClientRpc(string cardToClient, ClientRpcParams clientRpcParams = default)
    {
        CardToClient _fromServer = JsonUtility.FromJson<CardToClient>(cardToClient);
        Card _cardInfo = _deckManager.CardRepo.GetCard(_fromServer._card);
        GwentCard _newCard = new GwentCard(_fromServer._unique, _cardInfo);
        _cardsInHandScreen.RemoveSingleCardFromHand(_newCard);
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
    public void PlayCardDuringTurnServerRpc(string cardToServer, int cardSlot, EnumUnitPlacement cardPlace, string _interactCards = null, ServerRpcParams serverRpcParams = default)
    {
        if (_turnManager == null || _turnManager.CurrentPhase != EnumGameplayPhases.Regular) return;

        var clientId = serverRpcParams.Receive.SenderClientId;
        var _play = _playersLogic.Find((logic) => logic.MyInfo.ID == clientId);

        if (!_play.TurnActive)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.InvalidInput, "Wrong player sending inputs to server.", _play.MyInfo.Username);
            return;
        }

        CardToClient _cardFromClient = JsonUtility.FromJson<CardToClient>(cardToServer);

        if (!ValidateCardFromServer(_cardFromClient._card, cardSlot, _play.CardsInHand))
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.InvalidInput, "Invalid Card sent from client to server.", _play.MyInfo.Username);
            return;
        }

        Card _card = _deckManager.CardRepo.GetCard(_cardFromClient._card);
        
        if (!ValidatePlayedMinionPlacement(_card, cardPlace))
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.InvalidInput, $"Invalid placement for this card: {_card.id} {cardPlace}", _play.MyInfo.Username);
            return;
        }

        GwentCard _gwentCard = new GwentCard(_cardFromClient._unique, _card);
        var _continue = HandleLogicFromPlayedCard(_gwentCard, clientId, _cardFromClient._card, cardPlace, cardSlot, _play, _interactCards);

        if (!_continue) return;

        //Score
        UpdateScores();

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

    [ServerRpc(RequireOwnership = false)]
    public void SelectedGraveyardCardServerRpc(string cardToServer, int cardSlot, EnumUnitPlacement cardPlace, ServerRpcParams serverRpcParams = default)
    {
        if (_turnManager == null || _turnManager.CurrentPhase != EnumGameplayPhases.Regular) return;

        var clientId = serverRpcParams.Receive.SenderClientId;
        var _play = _playersLogic.Find((logic) => logic.MyInfo.ID == clientId);

        if (!_play.TurnActive)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.InvalidInput, "Wrong player sending inputs to server.", _play.MyInfo.Username);
            return;
        }

        CardToClient _cardFromClient = JsonUtility.FromJson<CardToClient>(cardToServer);

        if (!ValidateCardFromServer(_cardFromClient._card, cardSlot, _play.CardsInGraveyard))
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.InvalidInput, "Invalid Card sent from client to server.", _play.MyInfo.Username);
            return;
        }

        Card _card = _deckManager.CardRepo.GetCard(_cardFromClient._card);

        if (!ValidatePlayedMinionPlacement(_card, cardPlace))
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.InvalidInput, $"Invalid placement for this card: {_card.id} {cardPlace}", _play.MyInfo.Username);
            return;
        }

        GwentCard _gwentCard = new GwentCard(_cardFromClient._unique, _card);
        _play.RemoveCardFromGraveyard(cardSlot);
        CardToClient[] cardNames = _play.ReturnCardIds(EnumCardListType.Graveyard);
        //var _json = JsonUtility.ToJson(new CardNames(cardNames));
        var _json = GeneralPurposeFunctions.ConvertArrayToJson(cardNames);
        UpdateGraveyardClientRpc(_json, _play.ClientRpcParams);
        var _continue = HandleLogicFromPlayedCard(_gwentCard, clientId, _cardFromClient._card, cardPlace, GlobalConstantValues.LOGIC_NULLINT, _play);

        if (!_continue) return;

        //Play other multistep cards
        PlayStoredMultiStepCardsFromPlayer(_play);
        CloseGraveyardUIClientRpc(_play.ClientRpcParams);

        //Score
        UpdateScores();
        _turnManager.EndRegularTurn(false);
    }
    #endregion Server RPC
}
