using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUICanvas : MonoBehaviour
{
    [Header("User UI Related")]
    [SerializeField] private RawImage _userLeader = null;
    [SerializeField] private TextMeshProUGUI _userUsername = null;
    [SerializeField] private Image _userTurnHighlight = null;

    [Header("Enemy UI Related")]
    [SerializeField] private RawImage _enemyLeader = null;
    [SerializeField] private TextMeshProUGUI _enemyUsername = null;
    [SerializeField] private Image _enemyTurnHighlight = null;

    [Header("Main UI Related")]
    [SerializeField] private GameObject _mulliganHolder = null;

    #region Parameters
    private Color _highlightOff = new Color(0f, 0f, 0f, 0f);
    private Color _highlightOn = new Color(1f, 1f, 1f, 1f);
    #endregion Parameters

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

        _userTurnHighlight.color = _highlightOff;
        _enemyTurnHighlight.color = _highlightOff;
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
}
