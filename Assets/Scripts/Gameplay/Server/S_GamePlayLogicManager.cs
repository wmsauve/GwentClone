using Unity.Netcode;
using UnityEngine;

public class S_GamePlayLogicManager : NetworkBehaviour
{
    private C_PlayerGamePlayLogic[] _playersLogic = null;
    private UI_MulliganScroll _mulliganScreen = null;

    private int _currentActive;

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
        _playersLogic = FindObjectsOfType<C_PlayerGamePlayLogic>();
        if(_playersLogic == null || _playersLogic.Length == 0)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "There is no player logic in the scene.");
            return;
        }

        //pick first player.
        _currentActive = Random.Range(0, _playersLogic.Length);
        SetPlayerTurnActive();

        S_DeckManagers deckManager = GetComponent<S_DeckManagers>();
        if(deckManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Your GameLogic manager and Deck manager should be on the same gameobject.");
            return;
        }

        for(int i = 0; i < _playersLogic.Length; i++)
        {
            var id = _playersLogic[i].gameObject.GetComponent<NetworkObject>().OwnerClientId;
            foreach(GwentPlayer player in deckManager.GwentPlayers)
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
                        var _json = JsonUtility.ToJson(cardNames);
                        Debug.LogWarning(cardNames.Length);
                        Debug.LogWarning(cardNames[0]);
                        ClientRpcParams _params = default;
                        _params.Send.TargetClientIds = new ulong[] { player.MyInfo.ID };
                        ShowMulliganScreenClientRpc(_json, _params);
                    }   
                }

                break;
            case EnumGameplayPhases.Regular:
                break;
            default:
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Error with ending phases.");
                break;
        }
    }

    private void SetPlayerTurnActive()
    {
        for(int i = 0; i < _playersLogic.Length; i++)
        {
            if (i == _currentActive) _playersLogic[i].TurnActive = true;
            else _playersLogic[i].TurnActive = false;
        }
    }

    [ClientRpc]
    private void ShowMulliganScreenClientRpc(string cardNames, ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;

        //string[] _cardNames = JsonUtility.FromJson<string[]>(cardNames);

        Debug.LogWarning(cardNames + " yoyoyo.");

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

        _mulliganScreen.gameObject.SetActive(true);

    }
}
