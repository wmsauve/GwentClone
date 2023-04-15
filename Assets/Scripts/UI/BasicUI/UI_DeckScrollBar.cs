using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GwentClone.UI;

namespace GwentClone
{
    public class UI_DeckScrollBar : UI_ScrollView
    {

        [Header("Saving Deck List Related")]
        [SerializeField] private GameObject m_deckChangeCheckerPrefab = null;
        [SerializeField] private GameObject m_deckSettingsMenuPrefab = null;
        [SerializeField] private Transform m_mainCanvas = null;
        

        private List<UI_DeckButton> listOfDeckButtons = new List<UI_DeckButton>();

        protected override void Awake()
        {
            base.Awake();

            if (m_deckChangeCheckerPrefab == null)
            {
                Debug.LogWarning("You didn't add a prefab for checking if you are sure about changing decks without saving.");
            }

            if(m_deckSettingsMenuPrefab == null || m_mainCanvas == null)
            {
                Debug.LogWarning("You did not include a prefab for the deck settings menu.");
            }

            if (listOfDeckButtons == null)
            {
                listOfDeckButtons = new List<UI_DeckButton>();
            }
        }

        public void AddDeck(Deck _newDeck)
        {
            var _newBtn = Instantiate(m_buttonPrefab, m_content);
            var _btnComp = _newBtn.GetComponentInChildren<UI_DeckButton>();
            if (_btnComp == null)
            {
                Debug.LogWarning("Your button doesn't have the main functionality component.");
                return;
            }
            _btnComp.InitializeDeckButton(_newDeck, this);

            listOfDeckButtons.Add(_btnComp);
        }

        public void TriggerDeckNotSavedYetWarning(MonoBehaviour whoSelected)
        {
            var saveChecker = Instantiate(m_deckChangeCheckerPrefab, transform);
            var checkerComponent = saveChecker.GetComponent<SaveDeckCheckerButtons>();
            if(checkerComponent == null)
            {
                Debug.LogWarning("Find out why your prefab for the checker doesn't have its component.");
                return;
            }
            checkerComponent.InitializeTheChecker(whoSelected);
        }

        public void TurnOfHighlightOfPreviousButton()
        {
            foreach(UI_DeckButton button in listOfDeckButtons)
            {
                if (button.IsSelected)
                {
                    button.SetThisButtonAsOff();
                    return;
                }
            }
        }

        public void RevertCurrentButtonCachedName()
        {
            foreach (UI_DeckButton button in listOfDeckButtons)
            {
                if (button.IsSelected)
                {
                    button.SetCachedName();
                    return;
                }
            }
        }

        public void OpenDeckMenu(Deck whichDeck)
        {
            if (m_deckSettingsMenuPrefab == null || m_mainCanvas == null) return;

            var _menu = Instantiate(m_deckSettingsMenuPrefab, m_mainCanvas);
            var _settingsComponent = _menu.GetComponent<DeckSettingsMenu>();
            if(_settingsComponent == null)
            {
                Debug.LogWarning("Find out why you don't have the settings component on your prefab.");
                return;
            }

            _settingsComponent.InitializeOnPopup(whichDeck);
        }
    }

}
