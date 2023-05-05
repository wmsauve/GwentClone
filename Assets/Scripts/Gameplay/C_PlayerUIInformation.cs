using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class C_PlayerUIInformation : NetworkBehaviour
{
    private GameplayUICanvas _canvas = null;

    private S_DeckManagers _deckManager = null;

    private GwentPlayer _myInfo = null;
    

    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            S_DeckManagers[] _manager = FindObjectsOfType<S_DeckManagers>();
            if (_manager.Length > 1)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Don't put 2 of these components here.");
                return;
            }

            _deckManager = _manager[0];
        }

        if (IsClient)
        {
            GameplayUICanvas[] _canvases = FindObjectsOfType<GameplayUICanvas>();
            if(_canvases.Length > 1)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Don't put 2 of these components here.");
                return;
            }

            _canvas = _canvases[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if(_myInfo == null)
            {
                var id = gameObject.GetComponent<NetworkObject>().OwnerClientId;
                GwentPlayer _info = _deckManager.GwentPlayers.Find((player) => player.ID == id);
                if (_info != null) _myInfo = _info;
            }

            if (_canvas == null || _myInfo == null) return;
            AddInformationToUIClientRpc(_myInfo.Username);
        }
    }

    [ClientRpc]
    private void AddInformationToUIClientRpc(string player)
    {
        if (IsOwner) _canvas.TestThisShit(player);

    }
}
