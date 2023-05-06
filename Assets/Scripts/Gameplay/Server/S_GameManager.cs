using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class S_GameManager : NetworkBehaviour
{
    [SerializeField] private S_TurnManager _turnManager = null;
    [SerializeField] private S_DeckManagers _deckManager = null;
    [SerializeField] private int _playersInGame = 2;
    [SerializeField] private bool _useTestDecks = false;

    private List<string> _usernames = new List<string>(); 

    private void Start()
    {        
        if(_turnManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "turn manager");
            return;
        }

        if (_deckManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "deck manager");
            return;
        }

        if (NetworkManager.Singleton == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "no Network Manager");
            return;
        }

        NetworkManager.Singleton.OnClientDisconnectCallback += DisconnectGame;
        NetworkManager.Singleton.OnClientConnectedCallback += ConnectedClient;
    }

    private void DisconnectGame(ulong id)
    {
        if (IsServer)
        {
            _turnManager.GameStart = false;
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, "Player Discconnected: " + id);
        }
    }

    private void ConnectedClient(ulong id)
    {
        if (IsClient)
        {
            _deckManager.RunCardRepoCheck();
        }

        if (_useTestDecks && _deckManager != null)
        {
            if (IsServer)
            {
                if (_turnManager.GameStart)
                {
                    //TODO: observer client?
                    
                }
                else
                {
                    var _newPlayer = "placeholder " + id;
                    GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, "Player Connected: " + _newPlayer);
                    _deckManager.AddNewGwentPlayer(_newPlayer, id);
                }

                //TODO: Find where the game ought to start.
                if(_deckManager.GwentPlayers.Count == _playersInGame) _turnManager.GameStart = true;
            }

        }

        if (!_useTestDecks && IsClient)
        {
            if(GameInstance.Instance == null)
            {
                Debug.LogWarning("You are not using test decks but also not logging it.");
                return;
            }
            UsernameToFromClientServerRpc(GameInstance.Instance.User.username);
        }
    }

    public void EndGameResult(string username)
    {
        _turnManager.GameStart = false;
        GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, "Game Over. Winner: " + username);
    }


    private void Update()
    {
        if (!IsServer) return;

        if (_useTestDecks) return;
        
        if (_turnManager == null || _deckManager == null) return;

        //TODO: This should be triggered after both players' decks have been loaded into the server from their database.
        if (!_turnManager.GameStart)
        {
            if(NetworkManager.Singleton.ConnectedClientsList.Count == _playersInGame)
            {
                StartCoroutine(BeginStartingGame()); 
            }
        }
    }

    //Need to test API call and getting decks.
    private IEnumerator BeginStartingGame()
    {
        var _apiManager = APIManager.Instance;
        var _apiCall = _apiManager.API_URL + _apiManager.API_ENDPOINT_FETCHDECK;

        //yield return StartCoroutine(_apiManager.PostRequest(_apiCall, _firstPlayer, EnumAPIType.fetchuserdeck));

        //yield return StartCoroutine(_apiManager.PostRequest(_apiCall, _secondPlayer, EnumAPIType.fetchuserdeck));

        _turnManager.GameStart = true;
        yield return null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void UsernameToFromClientServerRpc(string username)
    {
        _usernames.Add(username);
    }
}
