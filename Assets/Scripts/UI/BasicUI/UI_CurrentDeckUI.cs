using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GwentClone
{
    public class UI_CurrentDeckUI : UI_InitializeFromManager
    {
        [Header("Major Panels Related")]
        [SerializeField] private GameObject m_noDeckUI = null;
        [SerializeField] private GameObject m_hasDeckUI = null;

        [Header("Individual Components Related")]
        [SerializeField] private Button m_createFirstDeck = null;
        [SerializeField] private UI_DeckScrollBar m_decksList = null;

        protected override void InitializeThisUIComp()
        {
            if(m_hasDeckUI == null || m_noDeckUI == null || m_decksList == null)
            {
                Debug.LogWarning("You don't have the UIs added to this component to show your decks.");
                return;
            }

            if(MainMenu_DeckManager.MyDecks.Count == 0) TurnOnHasDeck(false);
            else TurnOnHasDeck(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_createFirstDeck == null)
            {
                Debug.LogWarning("You didn't add the button to create your first deck.");
                return;
            }

            m_createFirstDeck.onClick.AddListener(CreateFirstDeck);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_createFirstDeck.onClick.RemoveListener(CreateFirstDeck);
        }

        private void InitializeNoDeckUI()
        {

        }

        private void InitializeHasDeckUI()
        {

        }

        private void TurnOnHasDeck(bool hasDeck)
        {
            if (hasDeck)
            {
                m_hasDeckUI.SetActive(true);
                m_noDeckUI.SetActive(false);
            }
            else
            {
                m_hasDeckUI.SetActive(false);
                m_noDeckUI.SetActive(true);
            }
        }

        private void CreateFirstDeck()
        {
            var _newDeck = MainMenu_DeckManager.AddDeck();
            m_decksList.AddDeck(_newDeck);
            TurnOnHasDeck(true);
        }
    }

}

