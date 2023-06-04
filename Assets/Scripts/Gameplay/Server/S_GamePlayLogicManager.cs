using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    private List<C_PlayerGamePlayLogic> _playersLogic = new List<C_PlayerGamePlayLogic>();
    private UI_MulliganScroll _mulliganScreen = null;
    private C_PlayerCardsUIManager _cardsInHandScreen = null;
    private C_ZonesManager _zoneManager = null;
    private S_DeckManagers _deckManager = null;
    private S_TurnManager _turnManager = null;

    private int _currentActive;
    private EnumGameplayPhases _currentPhase = EnumGameplayPhases.CoinFlip;

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

        _playersLogic = _getLogic.ToList();

        //pick first player.
        _currentActive = Random.Range(0, _playersLogic.Count);
        SetPlayerTurnActive();
        
        _deckManager = GetComponent<S_DeckManagers>();
        _turnManager = GetComponent<S_TurnManager>();
        
        if (_deckManager == null || _turnManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Your GameLogic manager and Deck/Turn manager should be on the same gameobject.");
            return;
        }
        
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

        if (IsClient)
        {
            _zoneManager = GetComponent<C_ZonesManager>();
            if (_zoneManager == null)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Your GameLogic manager and Deck/Turn manager should be on the same gameobject.");
                return;
            }
        }
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
                    foreach (C_PlayerGamePlayLogic player in _playersLogic)
                    {
                        if (_currentPhase == EnumGameplayPhases.Mulligan)
                        {
                            player.EndMulliganPhase();
                            string[] cardNames = player.ReturnCardIds();
                            var _json = JsonUtility.ToJson(new CardNames(cardNames));
                            StartFirstRegularTurnClientRpc(_json, player.ClientRpcParams);
                        }
                        else
                        {
                            _currentActive++;
                            if (_currentActive == _playersLogic.Count) _currentActive = 0;
                            SetPlayerTurnActive();
                        }
                    }

                    _currentPhase = _phase;
                }

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
            if (i == _currentActive) _playersLogic[i].TurnActive = true;
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
            Debug.LogWarning(cardSlot);
            Debug.LogWarning(whichCard);
            if (card.id == cardName && cardSlot == whichCard) return true;
            whichCard++;
        }
        return false;
    }

    [ClientRpc]
    private void ShowMulliganScreenClientRpc(string cardNames, ClientRpcParams clientRpcParams = default)
    {
        _deckManager = GetComponent<S_DeckManagers>();
        if (_deckManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Your GameLogic manager and Deck manager should be on the same gameobject.");
            return;
        }
        Debug.LogWarning(cardNames);

        var _mulligan = Resources.FindObjectsOfTypeAll<UI_MulliganScroll>();
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
    public void StartFirstRegularTurnClientRpc(string cardNames, ClientRpcParams clientRpcParams = default)
    {
        _mulliganScreen.gameObject.SetActive(false);

        var _cardInHand = Resources.FindObjectsOfTypeAll<C_PlayerCardsUIManager>();
        if (_cardInHand == null || _cardInHand.Length > 1)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "You should only have one card in hand object.");
            return;
        }
        if (_cardInHand.Length == 0)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You don't have the screen added for Card in hand phase.");
            return;
        }

        List<Card> _cards = new List<Card>();
        CreateListOfCardsFromString(ref _cards, cardNames);
        _cardsInHandScreen = _cardInHand[0];
        _cardsInHandScreen.InitializeHand(_cards, this);
        _cardsInHandScreen.gameObject.SetActive(true);
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
        var _cardData = _deckManager.CardRepo.GetCard(cardName);

        if (IsOwner) _zoneManager.AddCardToZone(_cardData, cardPlace, true);
        else _zoneManager.AddCardToZone(_cardData, cardPlace, false);
    }


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
    public void PlayCardDuringTurnServerRpc(string cardName, int cardSlot, EnumUnitPlacement cardPlace, ServerRpcParams serverRpcParams = default)
    {
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

        Debug.LogWarning(cardName + " this card getting played.");
        PlacePlayedCardClientRpc(cardName, cardPlace);
    }
}
