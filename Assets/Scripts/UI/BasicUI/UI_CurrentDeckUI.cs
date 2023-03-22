using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GwentClone.UI
{
    public class UI_CurrentDeckUI : UI_InitializeFromManager
    {
        [Header("Major Panels Related")]
        [SerializeField] private GameObject m_noDeckUI = null;
        [SerializeField] private GameObject m_hasDeckUI = null;
        [SerializeField] private GameObject m_cardListScrollPrefab = null;
        [SerializeField] private GameObject m_leaderPage = null;

        [Header("Individual Components Related")]
        [SerializeField] private Button m_createFirstDeck = null;
        [SerializeField] private UI_DeckScrollBar m_decksList = null;
        [SerializeField] private UI_DeckListManager m_deckListManager = null;
        [SerializeField] private UI_FillCardMenu m_selectableCardsManager = null;

        private Dictionary<string, GameObject> _deckScrolls = new Dictionary<string, GameObject>();
        private GameObject _currentWindow = null;


        protected override void InitializeThisUIComp()
        {
            if(m_hasDeckUI == null || m_noDeckUI == null || m_decksList == null || m_cardListScrollPrefab == null || m_deckListManager == null || m_leaderPage == null || m_selectableCardsManager == null)
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

            m_createFirstDeck.onClick.AddListener(OpenLeaderPage);
            GlobalActions.OnPressDeckChangeButton += SwitchCurrentDeck;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_createFirstDeck.onClick.RemoveListener(OpenLeaderPage);
            GlobalActions.OnPressDeckChangeButton -= SwitchCurrentDeck;
        }

        private void InitializeNoDeckUI()
        {

        }

        private void InitializeHasDeckUI()
        {

        }

        public void TurnOnHasDeck(bool hasDeck)
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

        public void CreateADeck(Leader leader)
        {
            if (m_cardListScrollPrefab == null) return;

            var _newDeck = MainMenu_DeckManager.AddDeck(leader);
            m_decksList.AddDeck(_newDeck);
            var newWindow = Instantiate(m_cardListScrollPrefab, m_hasDeckUI.transform);
            _deckScrolls.Add(_newDeck.DeckUID, newWindow);
            SwitchCurrentDeck(_newDeck);
            MainMenu_DeckManager.SwitchFocusedDeck(_newDeck);

            if (m_noDeckUI.activeSelf)
            {
                TurnOnHasDeck(true);
            }
            
        }

        public void OpenLeaderPage()
        {
            if (m_leaderPage.activeSelf)
            {
                Debug.LogWarning("This doesn't make sense. Leaderpage shouldn't be open yet.");
                return;
            }

            m_leaderPage.SetActive(true);
        }

        private void SwitchCurrentDeck(Deck deck)
        {
            if (_deckScrolls.ContainsKey(deck.DeckUID))
            {
                GameObject toActivate;
                toActivate = _deckScrolls[deck.DeckUID];

                if (toActivate == _currentWindow) return;

                toActivate.SetActive(true);
                if (_currentWindow != null) _currentWindow.SetActive(false);
                _currentWindow = toActivate;

                var content = toActivate.transform.GetChild(0).GetChild(0);
                if (content == null)
                {
                    Debug.LogWarning("check your prefab to see if the content is a child of a child.");
                    return;
                }

                if(MainMenu_DeckSaved.DeckChangedStatus == EnumDeckStatus.Changed)
                {
                    m_deckListManager.SetOldDeckListButtons();
                    MainMenu_DeckSaved.DeckChangedStatus = EnumDeckStatus.NotChanged;
                }

                m_selectableCardsManager.ResetListBasedOnFaction(deck.DeckLeader.factionType);
                m_deckListManager.SetCurrentContent(content);
            }
            else
            {
                Debug.LogWarning("Find out why you have a button activating this but no scroll for it.");
                return;
            }
            
        }

    }

}

