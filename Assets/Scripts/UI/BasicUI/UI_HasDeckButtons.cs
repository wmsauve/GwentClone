using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackendFunctionality;

namespace GwentClone.UI
{

    public class UI_HasDeckButtons : UI_InitializeFromManager, ISaveDependentComponent
    {
        [System.Serializable]
        private struct PassDeckSaveToServer
        {
            public string username;
            public Deck[] decks;

            [System.Serializable]
            public struct Deck
            {
                public string name;
                public string leaderName;
                public bool isCurrent;
                public Card[] cards;
            }

            [System.Serializable]
            public struct Card
            {
                public string name;
            }
            
        }

        [Header("Individual Components Related")]
        [SerializeField] private Button m_saveDeckBtn = null;
        [SerializeField] private Button m_newDeckBtn = null;
        [SerializeField] private UI_DeckScrollBar m_deckButtonScroll = null;
        [SerializeField] private UI_CurrentDeckUI m_createDeckFunctionality = null;

        private bool resolveNotSaved = false;
        private Coroutine saveCoroutine;

        private void Update()
        {
            if (!resolveNotSaved) return;
            m_createDeckFunctionality.OpenLeaderPage();
            resolveNotSaved = false;
        }

        public override void InitializeThisUIComp()
        {
            if (m_saveDeckBtn == null || m_newDeckBtn == null || m_createDeckFunctionality == null || m_deckButtonScroll == null)
            {
                Debug.LogWarning("You are not initializing the save deck or new deck buttons.");
                return;
            }

            m_saveDeckBtn.interactable = false;

        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_saveDeckBtn == null || m_newDeckBtn == null)
            {
                Debug.LogWarning("You are not initializing the save deck or new deck buttons.");
                return;
            }

            m_saveDeckBtn.onClick.AddListener(SaveButtonFunctionality);
            m_newDeckBtn.onClick.AddListener(NewDeckFunctionality);
            GlobalActions.OnDeckChanged += ListenToChangeInDeckStatus;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_saveDeckBtn.onClick.RemoveListener(SaveButtonFunctionality);
            m_newDeckBtn.onClick.RemoveListener(NewDeckFunctionality);
            GlobalActions.OnDeckChanged -= ListenToChangeInDeckStatus;

            if(saveCoroutine != null)
            {
                StopCoroutine(saveCoroutine);
                saveCoroutine = null;
            }
        }

        private void SaveButtonFunctionality()
        {
            var fullAPI = APIManager.Instance.API_URL + APIManager.Instance.API_ENDPOINT_SAVEDECK;
            StartCoroutine(SaveToServer(fullAPI));
        }

        private IEnumerator SaveToServer(string url)
        {
            var _decksToSave = GenerateBodyForServer();
            StartCoroutine(APIManager.Instance.PostRequest(url, _decksToSave, EnumAPIType.savedeck));
            yield return null;

            MainMenu_DeckManager.DeckSaved();
            MainMenu_DeckSaved.DeckChangedStatus = EnumDeckStatus.NotChanged;
        }

        private void NewDeckFunctionality()
        {
            if (MainMenu_DeckSaved.DeckChangedStatus == EnumDeckStatus.Changed)
            {
                m_deckButtonScroll.TriggerDeckNotSavedYetWarning(this);
                return;
            }
            m_createDeckFunctionality.OpenLeaderPage();
        }

        private void ListenToChangeInDeckStatus(EnumDeckStatus status)
        {
            switch (status)
            {
                case EnumDeckStatus.Changed:
                    m_saveDeckBtn.interactable = true;
                    break;
                case EnumDeckStatus.NotChanged:
                    m_saveDeckBtn.interactable = false;
                    break;
                case EnumDeckStatus.Resolving:
                    break;
                default:
                    Debug.LogWarning("Check to see if the Enum is correct.");
                    break;
            }
        }

        public void OnResolveSaveCheck()
        {
            m_deckButtonScroll.RevertCurrentButtonCachedName();
            resolveNotSaved = true;
        }

        private PassDeckSaveToServer GenerateBodyForServer()
        {
            PassDeckSaveToServer _newSave = new PassDeckSaveToServer();
            List<Deck> _decks = MainMenu_DeckManager.MyDecks;
            _newSave.username = GameInstance.Instance.User.username;
            _newSave.decks = new PassDeckSaveToServer.Deck[_decks.Count];

            for(int i = 0; i < _newSave.decks.Length; i++)
            {
                var _copyDeck = new PassDeckSaveToServer.Deck();
                var _prevDeck = _decks[i];

                _copyDeck.name = _prevDeck.DeckName;
                _copyDeck.leaderName = _prevDeck.DeckLeader.id;
                _copyDeck.isCurrent = _prevDeck.IsCurrentDeck;

                var _prevCards = _prevDeck.Cards;
                var _copyCards = new PassDeckSaveToServer.Card[_prevCards.Count];

                for(int j = 0; j < _prevCards.Count; j++)
                {
                    _copyCards[j].name = _prevCards[j].id;
                }

                _copyDeck.cards = _copyCards;
                _newSave.decks[i] = _copyDeck;
            }

            return _newSave;
        }
    }

}

