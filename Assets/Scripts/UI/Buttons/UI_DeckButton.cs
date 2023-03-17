using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GwentClone
{
    public class UI_DeckButton : MonoBehaviour
    {
        [Header("Buttons Related")]
        [SerializeField] private Button buttonComp = null;
        [SerializeField] private Button acceptButton = null;
        [SerializeField] private Button cancelButton = null;

        [Header("Change Name Object Related")]
        [SerializeField] private GameObject changeNameObject = null;
        [SerializeField] private TMP_InputField changeNameField = null;

        [Header("Deck Name Object Related")]
        [SerializeField] private GameObject deckNameObject = null;
        [SerializeField] private TextMeshProUGUI deckNameText = null;

        //Selection related
        private RightClick rightClickComp = null;
        private Deck whichDeck;
        [Header("Highlight selected button.")]
        [SerializeField] private Image m_backGroundImage = null;
        private Color hiddenHighlight = new Color(0f, 0f, 0f, 0f);
        private Color shownHighlight = new Color(0f, 0f, 0f, 1f);
        private bool isSelected = false;
        public bool IsSelected { get { return isSelected; } }

        private UI_DeckScrollBar managerReference = null;

        //Save related
        private string cachedDeckName;
        private bool notSaveResolved = false;
        private bool markedForSwitch = false;

        private void Update()
        {
            if (!notSaveResolved) return;

            SelectFunctionality();
            notSaveResolved = false;
            markedForSwitch = false;
        }

        public void InitializeDeckButton(Deck newDeck, UI_DeckScrollBar manager)
        {

            if (buttonComp == null || changeNameObject == null || deckNameObject == null || changeNameField == null || deckNameText == null || acceptButton == null || cancelButton == null || m_backGroundImage == null)
            {
                Debug.LogWarning("Your deck button does not have all of its components initialized.");
                return;
            }

            var _name = newDeck.DeckName;
            deckNameText.text = _name;
            cachedDeckName = _name;
            changeNameObject.SetActive(false);
            deckNameObject.SetActive(true);
            whichDeck = newDeck;
            //Swap from previous deck.
            managerReference = manager;
            SetThisButtonAsSelected();
        }

        private void OnEnable()
        {
            if (buttonComp == null || changeNameObject == null || deckNameObject == null || changeNameField == null || deckNameText == null || acceptButton == null || cancelButton == null)
            {
                Debug.LogWarning("Your deck button does not have all of its components initialized.");
                return;
            }

            rightClickComp = GetComponent<RightClick>();
            if (rightClickComp == null)
            {
                Debug.LogWarning("You can't right click this button to change the name of your deck");
                return;
            }

            rightClickComp.rightClick.AddListener(BeginNameChange);
            acceptButton.onClick.AddListener(AcceptNameChange);
            cancelButton.onClick.AddListener(CancelNameChange);
            buttonComp.onClick.AddListener(SelectThisDeck);
            GlobalActions.OnNotSavingDeck += RevertCachedName;
        }

        private void OnDisable()
        {
            rightClickComp.rightClick.RemoveListener(BeginNameChange);
            acceptButton.onClick.RemoveListener(AcceptNameChange);
            cancelButton.onClick.RemoveListener(CancelNameChange);
            buttonComp.onClick.RemoveListener(SelectThisDeck);
            GlobalActions.OnNotSavingDeck -= RevertCachedName;
        }

        private void CancelNameChange()
        {
            changeNameObject.SetActive(false);
            deckNameObject.SetActive(true);
            buttonComp.interactable = true;
        }

        private void RevertCachedName(bool revertDeck)
        {
            if (whichDeck.DeckUID != MainMenu_DeckManager.GetCurrentDeckGUID())
            {
                if (markedForSwitch) notSaveResolved = true;
            }
            else
            {
                if (!revertDeck) return;
                deckNameText.text = cachedDeckName;
            }
        }

        private void AcceptNameChange()
        {
            var _newDeckName = changeNameField.text.ToUpper().Trim();
            cachedDeckName = deckNameText.text.ToUpper().Trim();
            deckNameText.text = _newDeckName;
            whichDeck.SetDeckName(_newDeckName);
            changeNameObject.SetActive(false);
            deckNameObject.SetActive(true);
            buttonComp.interactable = true;

            var deckStatus = MainMenu_DeckManager.RunCheckForDeckChange();
            MainMenu_DeckSaved.DeckChangedStatus = deckStatus;
        }

        private void BeginNameChange()
        {
            if (whichDeck.DeckUID != MainMenu_DeckManager.GetCurrentDeckGUID()) return;

            changeNameObject.SetActive(true);
            deckNameObject.SetActive(false);
            buttonComp.interactable = false;
        }

        private void SelectThisDeck()
        {
            if (MainMenu_DeckSaved.DeckChangedStatus == EnumDeckStatus.Changed)
            {
                managerReference.TriggerDeckNotSavedYetWarning();
                markedForSwitch = true;
                return;
            }
            SelectFunctionality();
        }

        private void SelectFunctionality()
        {
            SetThisButtonAsSelected();
            MainMenu_DeckManager.SwitchFocusedDeck(whichDeck);
            GlobalActions.OnPressDeckChangeButton?.Invoke(whichDeck);
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
    }

}

