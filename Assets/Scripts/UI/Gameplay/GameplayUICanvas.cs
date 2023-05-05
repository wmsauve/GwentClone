using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUICanvas : MonoBehaviour
{
    [Header("User UI Related")]
    [SerializeField] private RawImage _userLeader = null;

    [Header("Enemy UI Related")]
    [SerializeField] private RawImage _enemyLeader = null;


    public void TestThisShit(string myguy)
    {
        Debug.LogWarning(myguy + " Hello my guy;");
    }

}
