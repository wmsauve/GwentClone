using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUICanvas : MonoBehaviour
{
    [Header("User UI Related")]
    [SerializeField] private RawImage _userLeader = null;
    [SerializeField] private TextMeshProUGUI _userUsername = null;
    [SerializeField] private Image _userTurnHighlight = null;
    [SerializeField] private TextMeshProUGUI _userTotalPower = null;
    [Tooltip("When losing health, you will lose from later index to 0th index. User crystals.")]
    [SerializeField] private Image[] _userGems = null;

    [Header("Enemy UI Related")]
    [SerializeField] private RawImage _enemyLeader = null;
    [SerializeField] private TextMeshProUGUI _enemyUsername = null;
    [SerializeField] private Image _enemyTurnHighlight = null;
    [SerializeField] private TextMeshProUGUI _enemyTotalPower = null;
    [Tooltip("When losing health, you will lose from later index to 0th index. Enemy crystals.")]
    [SerializeField] private Image[] _enemyGems = null;

    [Header("Main UI Related")]
    [SerializeField] private GameObject _mulliganHolder = null;

    #region Parameters
    private Color _highlightOff = new Color(0f, 0f, 0f, 0f);
    private Color _highlightOn = new Color(1f, 1f, 1f, 1f);
    #endregion Parameters

    private C_PlayerGamePlayLogic _myLogic;

    private void Awake()
    {
        if(_mulliganHolder == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Main Canvas is missing component reference.");
            return;
        }

        if(_userTurnHighlight == null || _enemyTurnHighlight == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You don't have highlights for whoever's turn.");
            return;
        }

        if(_userTotalPower == null || _enemyTotalPower == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You don't have total score references.");
            return;
        }

        if(_userGems == null || _enemyGems == null || _enemyGems.Length == 0 || _userGems.Length == 0)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You don't have gem components added.");
            return;
        }

        _userTurnHighlight.color = _highlightOff;
        _enemyTurnHighlight.color = _highlightOff;

        _userTotalPower.text = 0.ToString();
        _enemyTotalPower.text = 0.ToString();
    }

    public void InitializeUI(string username, Sprite leaderSprite, EnumGameplayPlayerRole role)
    {
        switch (role)
        {
            case EnumGameplayPlayerRole.Player:
                _userLeader.texture = leaderSprite.texture;
                _userUsername.text = username;
                break;
            case EnumGameplayPlayerRole.Opponent:
                _enemyLeader.texture = leaderSprite.texture;
                _enemyUsername.text = username;
                break;
            default:
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Incorrect role for updating UI.");
                break;
        }
    }
    
    public void SetCrystals(int playerLives, int opponentLives)
    {
        DecrementCrystalUI(playerLives, _userGems);
        DecrementCrystalUI(opponentLives, _enemyGems);
    }

    private void DecrementCrystalUI(int lives, Image[] images)
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (i < lives) images[i].gameObject.SetActive(true);
            else images[i].gameObject.SetActive(false);
        }
    }

    public void ToggleMulliganScreenOff()
    {
        if (_mulliganHolder.activeSelf) _mulliganHolder.SetActive(false);
    }

    public void SetActivePlayer(bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            _enemyTurnHighlight.color = _highlightOff;
            _userTurnHighlight.color = _highlightOn;
        }
        else
        {
            _enemyTurnHighlight.color = _highlightOn;
            _userTurnHighlight.color = _highlightOff;
        }
    }

    public void SetNewScores(S_GamePlayLogicManager.MatchScores.ScoresToClient[] _newScores)
    {
        if (_myLogic == null) _myLogic = GeneralPurposeFunctions.GetPlayerLogicReference();

        ulong _myId = _myLogic.ReturnID();

        for(int i = 0; i < _newScores.Length; i++)
        {
            var _scores = _newScores[i];
            int sum = _scores._front + _scores._ranged + _scores._siege;
            if (_scores._id == _myId) _userTotalPower.text = sum.ToString();
            else _enemyTotalPower.text = sum.ToString();
        }
    }
}
