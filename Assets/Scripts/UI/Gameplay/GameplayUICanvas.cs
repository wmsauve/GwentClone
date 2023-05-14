using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUICanvas : MonoBehaviour
{
    [Header("User UI Related")]
    [SerializeField] private RawImage _userLeader = null;
    [SerializeField] private TextMeshProUGUI _userUsername = null;

    [Header("Enemy UI Related")]
    [SerializeField] private RawImage _enemyLeader = null;
    [SerializeField] private TextMeshProUGUI _enemyUsername = null;

    [Header("Main UI Related")]
    [SerializeField] private GameObject _mulliganHolder = null;

    private EnumGameplayPhases _cacheCurrentPhase;


    private void Awake()
    {
        if(_mulliganHolder == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Main Canvas is missing component reference.");
            return;
        }
    }

    private void OnEnable()
    {
        GlobalActions.OnPhaseChange += DetectPhase;
    }

    private void OnDisable()
    {
        GlobalActions.OnPhaseChange -= DetectPhase;
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

    private void DetectPhase(EnumGameplayPhases phase)
    {
        switch (phase) 
        {
            case EnumGameplayPhases.Regular:
                if(_cacheCurrentPhase == EnumGameplayPhases.Mulligan)
                {
                    _mulliganHolder.SetActive(false);
                }
                break;

        }
        _cacheCurrentPhase = phase;

    }

}
