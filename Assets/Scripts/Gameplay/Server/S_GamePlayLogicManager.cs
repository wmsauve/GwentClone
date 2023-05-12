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
    private S_DeckManagers _deckManager = null;

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
        if (_deckManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Your GameLogic manager and Deck manager should be on the same gameobject.");
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
                            CloseMulliganScreenClientRpc(player.ClientRpcParams);
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
        CardNames _cardNames = JsonUtility.FromJson<CardNames>(cardNames);
        foreach (string name in _cardNames._cards)
        {
            Card newCard = _deckManager.CardRepo.GetCard(name);
            if(newCard == null)
            {
                Debug.LogWarning(name);
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Invalid card name from server to client.");
                return;
            }

            _cards.Add(newCard);
        }

        _mulliganScreen.InitializeMulliganCards(_cards, this, GlobalConstantValues.GAME_MULLIGANSAMOUNT);

        _mulliganScreen.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void CloseMulliganScreenClientRpc(ClientRpcParams clientRpcParams = default)
    {
        _mulliganScreen.gameObject.SetActive(false);
    }

    [ClientRpc]
    public void WaitingForOtherPlayerScreenClientRpc(ClientRpcParams clientRpcParams = default)
    {
        _mulliganScreen.FinishMulligan();
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


    [ServerRpc(RequireOwnership = false)]
    public void MulliganACardServerRpc(string cardName, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        var _play = _playersLogic.Find((logic) => logic.MyInfo.ID == clientId);
        List<Card> _cards = _play.CardsInHand;
        int cardCheck = 0;

        foreach(Card card in _cards)
        {
            if (card.id == cardName) break;
            else cardCheck++;
        }

        if(cardCheck == _cards.Count)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.InvalidInput, "Invalid Card sent from client to server.", _play.MyInfo.Username);
            return;
        }

        string _newCard = _play.MulliganCard(cardName);
        if (_newCard == string.Empty) return;
        MulliganACardClientRpc(_newCard, _play.Mulligans, _play.ClientRpcParams);
    }
}
