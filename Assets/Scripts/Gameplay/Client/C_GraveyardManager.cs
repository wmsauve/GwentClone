using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class C_GraveyardManager : MonoBehaviour
{
    [SerializeField] private Button m_toggleGYButton = null;
    [SerializeField] private UI_GraveyardCards m_graveYardUI = null;

    private GameObject m_graveYardContainer = null;

    private void Start()
    {
        var init = m_toggleGYButton.gameObject.GetComponent<UI_OnButtonHover>();
        if (m_toggleGYButton == null || init == null || m_graveYardUI == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Missing Graveyard Components in Manager.");
            return;
        }

        m_graveYardContainer = m_graveYardUI.gameObject;
    }

    private void OnEnable()
    {
        m_toggleGYButton.onClick.AddListener(ToggleGraveyardUI);
    }

    private void OnDisable()
    {
        m_toggleGYButton.onClick.RemoveListener(ToggleGraveyardUI);
    }

    public void ToggleGraveyardUI()
    {
        if (m_graveYardContainer == null) return;
        m_graveYardContainer.SetActive(true);
    }

    public void PassCardsToGraveyard(List<GwentCard> _cards)
    {
        m_graveYardUI.AddCardsToGraveyard(_cards);
    }

    public void TurnOffGraveyardFromServer()
    {
        m_graveYardUI.CloseGraveyard();
    }
}
