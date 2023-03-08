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
        [SerializeField] private TextMeshProUGUI changeNameField = null;

        [Header("Deck Name Object Related")]
        [SerializeField] private GameObject deckNameObject = null;
        [SerializeField] private TextMeshProUGUI deckNameText = null;

        private RightClick rightClickComp = null;
        private Deck whichDeck;

        public void InitializeDeckButton(Deck newDeck)
        {

            if (buttonComp == null || changeNameObject == null || deckNameObject == null || changeNameField == null || deckNameText == null || acceptButton == null || cancelButton == null)
            {
                Debug.LogWarning("Your deck button does not have all of its components initialized.");
                return;
            }

            rightClickComp = GetComponent<RightClick>();
            if(rightClickComp == null)
            {
                Debug.LogWarning("You can't right click this button to change the name of your deck");
                return;
            }

            rightClickComp.rightClick.AddListener(BeginNameChange);
            acceptButton.onClick.AddListener(AcceptNameChange);
            cancelButton.onClick.AddListener(CancelNameChange);
            buttonComp.onClick.AddListener(SelectThisDeck);

            var _name = newDeck.DeckName;
            deckNameText.text = _name;
            changeNameObject.SetActive(false);
            deckNameObject.SetActive(true);
            whichDeck = newDeck;
        }

        private void CancelNameChange()
        {
            changeNameObject.SetActive(false);
            deckNameObject.SetActive(true);
            buttonComp.interactable = true;
        }

        private void AcceptNameChange()
        {
            var _newDeckName = changeNameField.text.ToUpper().Trim();
            deckNameText.text = _newDeckName;
            whichDeck.SetDeckName(_newDeckName);
            changeNameObject.SetActive(false);
            deckNameObject.SetActive(true);
            buttonComp.interactable = true;

            var deckStatus = MainMenu_DeckManager.RunCheckForDeckChange();
            switch (deckStatus)
            {
                case EnumDeckStatus.Changed:
                    Debug.LogWarning("Deck changed");
                    break;
                case EnumDeckStatus.NotChanged:
                    Debug.LogWarning("Deck no longer changed");
                    break;
            }
        }

        private void BeginNameChange()
        {
            changeNameObject.SetActive(true);
            deckNameObject.SetActive(false);
            buttonComp.interactable = false;
        }

        private void SelectThisDeck()
        {
            MainMenu_DeckManager.SwitchFocusedDeck(whichDeck);
        }
    }

}

