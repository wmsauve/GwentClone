using UnityEngine;
using UnityEngine.UI;

public class UI_LeaderPanel : UI_ScrollView
{
    [Header("Specific Objects Related")]
    [SerializeField] private GameObject m_mainPanel = null;
    [SerializeField] private Button m_cancelButton = null;
    [SerializeField] private UI_CurrentDeckUI m_createDeckManager = null;

    public override void InitializeThisUIComp()
    {
        if(m_mainPanel == null || m_cancelButton == null || m_createDeckManager == null)
        {
            Debug.LogWarning("You didn't add the correct objects or components to initialize this component.");
            return;
        }

        AddLeaderButtons();
        m_mainPanel.SetActive(false);
    }

    private void AddLeaderButtons()
    {
        var leaders = GameInstance.Instance.CardRepo.AllLeaders;

        foreach(Leader leader in leaders)
        {
            var _newButton = Instantiate(m_buttonPrefab, m_content);
            var _buttonComp = _newButton.GetComponentInChildren<UI_LeaderButton>();
            if(_buttonComp == null)
            {
                Debug.LogWarning("Find out why you don't have the button comp on your Leader button.");
                return;
            }

            _buttonComp.InitializeButton(leader, m_createDeckManager, this);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (m_cancelButton == null) return;
        m_cancelButton.onClick.AddListener(CloseLeaderPanel);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (m_cancelButton == null) return;
        m_cancelButton.onClick.RemoveListener(CloseLeaderPanel);
    }

    public void CloseLeaderPanel()
    {
        m_mainPanel.SetActive(false);
    }
}
