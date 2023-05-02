using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UI_LeaderButton : UI_ToolTipButton
{
    [SerializeField] private Image m_borderHighlight = null;
    [SerializeField] private Button m_buttonComp = null;
    [SerializeField] private TextMeshProUGUI m_leaderName = null;

    private Leader whichLeader = null;
    private UI_CurrentDeckUI createDeckManager = null;
    private UI_LeaderPanel manager = null;

    public void InitializeButton(Leader leader, UI_CurrentDeckUI createDeckMngr, UI_LeaderPanel mngr)
    {
        if(m_borderHighlight == null || m_buttonComp == null || m_leaderName == null)
        {
            Debug.LogWarning("Check that you put all your references in Leader button.");
            return;
        }

        m_leaderName.text = leader.id;

        m_borderHighlight.color = GeneralPurposeFunctions.ReturnColorBasedOnFaction(leader.factionType);

        whichLeader = leader;

        createDeckManager = createDeckMngr;
        manager = mngr;
    }

    private void OnEnable()
    {
        if (m_buttonComp == null) return;
        m_buttonComp.onClick.AddListener(SelectedHero);
    }

    private void OnDisable()
    {
        if (m_buttonComp == null) return;
        m_buttonComp.onClick.RemoveListener(SelectedHero);
    }

    private void SelectedHero()
    {
        if (whichLeader == null) return;
        createDeckManager.CreateADeck(whichLeader);
        GlobalActions.OnStopHoveredUIButton?.Invoke();
        manager.CloseLeaderPanel();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (whichLeader == null) return;
        GlobalActions.OnHoveredUIButton?.Invoke(null, whichLeader);
    }
}

