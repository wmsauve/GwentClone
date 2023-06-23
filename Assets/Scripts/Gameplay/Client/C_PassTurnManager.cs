using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_PassTurnManager : MonoBehaviour
{
    [SerializeField] private Button m_passTurnButton = null;

    private S_GamePlayLogicManager _manager = null;

    private void Start()
    {
        var init = m_passTurnButton.gameObject.GetComponent<UI_OnButtonHover>();
        if (m_passTurnButton == null || init == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Need PassTurn Button.");
            return;
        }
    }

    private void OnEnable()
    {
        _manager = GetComponent<S_GamePlayLogicManager>();
        if(_manager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "For simplicity's sake, place with GameManager.");
            return;
        }

        m_passTurnButton.onClick.AddListener(PassTurn);
    }

    private void OnDisable()
    {
        m_passTurnButton.onClick.RemoveListener(PassTurn);
    }

    private void PassTurn()
    {
        _manager.PassYourTurnServerRpc();
    }
}
