using Unity.Netcode;

public class C_PlayerUIInformation : NetworkBehaviour
{
    private GameplayUICanvas _canvas = null;

    private S_DeckManagers _deckManager = null;

    private ulong _myID;
    

    // Start is called before the first frame update
    void Start()
    {
        S_DeckManagers[] _manager = FindObjectsOfType<S_DeckManagers>();
        if (_manager.Length > 1)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Don't put 2 of these components here.");
            return;
        }

        _deckManager = _manager[0];

        if (IsServer)
        {
            _myID = gameObject.GetComponent<NetworkObject>().OwnerClientId;
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

    private void OnEnable()
    {
        GlobalActions.OnGameStart += GameStartHandler;
    }

    private void OnDisable()
    {
        GlobalActions.OnGameStart -= GameStartHandler;
    }

    private void GameStartHandler()
    {

        foreach(GwentPlayer player in _deckManager.GwentPlayers)
        {
            EnumGameplayPlayerRole _role;
            if (player.ID == _myID) _role = EnumGameplayPlayerRole.Player;
            else _role = EnumGameplayPlayerRole.Opponent;

            InitializeUIClientRpc(player.Username, player.Deck.DeckLeader.id, _role);
        }
    }

    [ClientRpc]
    private void InitializeUIClientRpc(string username, string leaderName, EnumGameplayPlayerRole role)
    {
        if (IsOwner)
        {
            var leader = _deckManager.CardRepo.GetLeader(leaderName);
            _canvas.InitializeUI(username, leader.cardImage, role);
        }
    }
}
