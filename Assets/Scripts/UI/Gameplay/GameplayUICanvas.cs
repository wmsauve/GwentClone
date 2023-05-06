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

}
