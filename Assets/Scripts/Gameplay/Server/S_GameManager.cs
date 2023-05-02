using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class S_GameManager : NetworkBehaviour
{
    [SerializeField] private S_TurnManager _turnManager = null;
    [SerializeField] private S_DeckManagers _deckManager = null;
    [SerializeField] private int _playersInGame = 2;
    [SerializeField] private bool _useTestDecks = false;


    private bool _firstPlayerConnected = false;

    private string _firstPlayer;
    private string _secondPlayer;

    private void Start()
    {        
        if(_turnManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "turn manager");
            return;
        }

        if(_deckManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "deck manager");
            return;
        }

        if(NetworkManager.Singleton == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "no Network Manager");
            return;
        }

        if (IsClient) UsernameToFromClientServerRpc(GameInstance.Instance.User.username);

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
        if (_turnManager == null || _deckManager == null) return;

        //TODO: This should be triggered after both players' decks have been loaded into the server from their database.
        if (IsServer && !_turnManager.GameStart)
        {
            if(NetworkManager.Singleton.ConnectedClientsList.Count == _playersInGame)
            {
                StartCoroutine(BeginStartingGame()); 
            }
        }
    }

    private IEnumerator BeginStartingGame()
    {
        var _apiManager = APIManager.Instance;
        var _apiCall = _apiManager.API_URL + _apiManager.API_ENDPOINT_FETCHDECK;

        yield return StartCoroutine(_apiManager.PostRequest(_apiCall, _firstPlayer, EnumAPIType.fetchuserdeck));

        yield return StartCoroutine(_apiManager.PostRequest(_apiCall, _secondPlayer, EnumAPIType.fetchuserdeck));

        _turnManager.GameStart = true;
    }

    [ServerRpc]
    private void UsernameToFromClientServerRpc(string username)
    {
        if (!_firstPlayerConnected)
        {
            _firstPlayer = username;
            _firstPlayerConnected = true;
            return;
        }

        _secondPlayer = username;
    }

}
