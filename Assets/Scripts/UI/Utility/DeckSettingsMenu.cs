using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckSettingsMenu : MonoBehaviour
{
    [Header("Components Of Menu")]
    [SerializeField] private TextMeshProUGUI m_deckNameHeader = null;
    [SerializeField] private TextMeshProUGUI m_leaderNameHeader = null;
    [SerializeField] private TMP_Dropdown m_leaderMenu = null;
    [SerializeField] private TMP_InputField m_newDeckNameInput = null;

    [Header("Buttons")]
    [SerializeField] private Button m_cancelBtn = null;
    [SerializeField] private Button m_saveBtn = null;

    private Deck _whichDeck;
    private UI_DeckButton _whichBtn;
    private List<TMP_Dropdown.OptionData> _leaderList = new List<TMP_Dropdown.OptionData>();

    private void OnEnable()
    {
        if(m_cancelBtn == null || m_saveBtn == null)
        {
            Debug.LogWarning("You didn't put buttons for deck settings.");
            return;
        }

        if(m_newDeckNameInput == null || m_leaderMenu == null)
        {
            Debug.LogWarning("You didn't put components for changing settings.");
            return;
        }

        m_cancelBtn.onClick.AddListener(CloseMenu);
        m_saveBtn.onClick.AddListener(SaveNewDeckSettings);
    }

    private void OnDisable()
    {
        m_cancelBtn.onClick.RemoveListener(CloseMenu);
        m_saveBtn.onClick.RemoveListener(SaveNewDeckSettings);
    }

    public void InitializeOnPopup(UI_DeckButton btn)
    {
        _whichBtn = btn;
        _whichDeck = _whichBtn.DeckRef;

        m_deckNameHeader.text = _whichDeck.DeckName;
        m_leaderNameHeader.text = _whichDeck.DeckLeader.id;

        List<Leader> leaders = GameInstance.Instance.CardRepo.GetLeadersByFaction(_whichDeck.DeckLeader.factionType);
        foreach(Leader leader in leaders)
        {
            var newOption = new TMP_Dropdown.OptionData();
            newOption.text = leader.id;
            _leaderList.Add(newOption);
        }

        m_leaderMenu.options = _leaderList;
    }

    private void SaveNewDeckSettings()
    {
        var newDeckName = m_newDeckNameInput.text;
        var leaderIndex = m_leaderMenu.value;
        var leaderOption = m_leaderMenu.options[leaderIndex];
        var newLeader = GameInstance.Instance.CardRepo.GetLeader(leaderOption.text);

        //Set settings.
        if(newLeader != null) MainMenu_DeckManager.SwitchLeaderOfCurrentDeck(newLeader);
        if(newDeckName.Length > 0) _whichBtn.AcceptNameChange(newDeckName);

        CloseMenu();
    }

    private void CloseMenu()
    {
        enabled = false;
        Destroy(gameObject);
    }

}

