using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GwentClone
{
    public class UI_HasDeckButtons : UI_InitializeFromManager, ISaveDependentComponent
    {
        [Header("Individual Components Related")]
        [SerializeField] private Button m_saveDeckBtn = null;
        [SerializeField] private Button m_newDeckBtn = null;
        [SerializeField] private UI_CurrentDeckUI m_currentDeckManager = null;
        [SerializeField] private UI_DeckScrollBar m_deckButtonScroll = null;

        private bool resolveNotSaved = false;

        private void Update()
        {
            if (!resolveNotSaved) return;

            m_currentDeckManager.CreateADeck();
            resolveNotSaved = false;
        }

        protected override void InitializeThisUIComp()
        {
            if (m_saveDeckBtn == null || m_newDeckBtn == null || m_currentDeckManager == null || m_deckButtonScroll == null)
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
        }

        private void SaveButtonFunctionality()
        {
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
            m_currentDeckManager.CreateADeck();
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
            if (MainMenu_DeckSaved.DeckChangedStatus != EnumDeckStatus.NotChanged) MainMenu_DeckSaved.DeckChangedStatus = EnumDeckStatus.NotChanged;
            resolveNotSaved = true;
        }
    }

}

