using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DeckButton : MonoBehaviour, ISaveDependentComponent
{
    [Header("Buttons Related")]
    [SerializeField] private Button buttonComp = null;

    [Header("Deck Name Object Related")]
    [SerializeField] private GameObject deckNameObject = null;
    [SerializeField] private TextMeshProUGUI deckNameText = null;

    //Selection related
    private Deck whichDeck;
    public Deck DeckRef { get { return whichDeck; } }
    [Header("Highlights Related")]
    [SerializeField] private Image m_backGroundImage = null;
    private Color hiddenHighlight = new Color(0f, 0f, 0f, 0f);
    private Color shownHighlight = new Color(0f, 0f, 0f, 1f);
    private bool isSelected = false;
    public bool IsSelected { get { return isSelected; } }

    private UI_DeckScrollBar managerReference = null;

    //Save related
    private bool notSaveResolved = false;

    private void Update()
    {
        if (!notSaveResolved) return;

        SelectFunctionality();
        notSaveResolved = false;
    }

    public void InitializeDeckButton(Deck newDeck, UI_DeckScrollBar manager)
    {

        if (buttonComp == null || deckNameObject == null || deckNameText == null || m_backGroundImage == null)
        {
            Debug.LogWarning("Your deck button does not have all of its components initialized.");
            return;
        }

        var _name = newDeck.DeckName;
        deckNameText.text = _name;
        deckNameObject.SetActive(true);
        whichDeck = newDeck;
        deckNameText.color = GeneralPurposeFunctions.ReturnColorBasedOnFaction(whichDeck.DeckLeader.factionType);

        //Swap from previous deck.
        managerReference = manager;
        SetThisButtonAsSelected();
    }

    private void OnEnable()
    {
        if (buttonComp == null || deckNameObject == null || deckNameText == null)
        {
            Debug.LogWarning("Your deck button does not have all of its components initialized.");
            return;
        }
        buttonComp.onClick.AddListener(SelectThisDeck);
    }

    private void OnDisable()
    {
        buttonComp.onClick.RemoveListener(SelectThisDeck);
    }

    public void AcceptNameChange(string newDeckName)
    {
        if (newDeckName.Length > 20)
        {
            GlobalActions.OnDisplayFeedbackInUI?.Invoke(GlobalConstantValues.MESSAGE_INPUTFIELDTOOLONG);
            return;
        }

        var _newDeckName = newDeckName.ToUpper().Trim();
        deckNameText.text = _newDeckName;
        whichDeck.SetDeckName(_newDeckName);
        deckNameObject.SetActive(true);
        buttonComp.interactable = true;

        var deckStatus = MainMenu_DeckManager.RunCheckForDeckChange();
        MainMenu_DeckSaved.DeckChangedStatus = deckStatus;
    }

    private void SelectThisDeck()
    {
        if(whichDeck == MainMenu_DeckManager.CurrentDeck)
        {
            managerReference.OpenDeckMenu(this);
            return;
        }

        if (MainMenu_DeckSaved.DeckChangedStatus == EnumDeckStatus.Changed)
        {
            managerReference.TriggerDeckNotSavedYetWarning(this);
            return;
        }
        SelectFunctionality();
    }

    private void SelectFunctionality()
    {
        SetThisButtonAsSelected();
        GlobalActions.OnPressDeckChangeButton?.Invoke(whichDeck);
        MainMenu_DeckManager.SwitchFocusedDeck(whichDeck);
    }

    private void SetThisButtonAsSelected()
    {
        managerReference.TurnOfHighlightOfPreviousButton();
        isSelected = true;
        m_backGroundImage.color = shownHighlight;
    }

    public void SetThisButtonAsOff()
    {
        isSelected = false;
        m_backGroundImage.color = hiddenHighlight;
    }
        
    public void SetCachedName()
    {
        whichDeck = MainMenu_DeckManager.RevertCurrentDeckToClone();
        deckNameText.text = MainMenu_DeckManager.GetUnsavedDeckName();
    }

    //Interface
    public void OnResolveSaveCheck()
    {
        managerReference.RevertCurrentButtonCachedName();
        notSaveResolved = true;
    }
}

