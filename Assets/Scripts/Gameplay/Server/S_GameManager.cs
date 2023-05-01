using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace GwentClone.Gameplay
{
    public class S_GameManager : NetworkBehaviour
    {
        [SerializeField] private S_TurnManager _turnManager = null;
        [SerializeField] private int _playersInGame = 2;

        private void Start()
        {        
            if(_turnManager == null)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "turn manager");
                return;
            }

            if(NetworkManager.Singleton == null)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "no Network Manager");
                return;
            }

            NetworkManager.Singleton.OnClientDisconnectCallback += DisconnectGame;
        }

        private void DisconnectGame(ulong id)
        {
            _turnManager.GameStart = false;
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, "Player Discconnected: " + id);
        }

        public void EndGameResult(string username)
        {
            _turnManager.GameStart = false;
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, "Game Over. Winner: " + username);
        }


        private void Update()
        {
            if (_turnManager == null) return;

            //TODO: This should be triggered after both players' decks have been loaded into the server from their database.
            if (IsServer && !_turnManager.GameStart)
            {
                if(NetworkManager.Singleton.ConnectedClientsList.Count == _playersInGame)
                {
                    _turnManager.GameStart = true;
                }
            }
        }
    }
}


