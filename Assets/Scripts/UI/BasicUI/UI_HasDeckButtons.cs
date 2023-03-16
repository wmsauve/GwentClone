using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GwentClone
{
    public class UI_HasDeckButtons : UI_InitializeFromManager
    {
        [Header("Individual Components Related")]
        [SerializeField] private Button m_saveDeckBtn = null;
        [SerializeField] private Button m_newDeckBtn = null;
        [SerializeField] private UI_CurrentDeckUI m_currentDeckManager = null;

        private bool resolveNotSaved = false;
        private Coroutine notSavedCoroutine;

        protected override void InitializeThisUIComp()
        {
            if (m_saveDeckBtn == null || m_newDeckBtn == null || m_currentDeckManager == null)
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
            GlobalActions.OnNotSavingDeck += ResolveUnsavedDeck;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_saveDeckBtn.onClick.RemoveListener(SaveButtonFunctionality);
            m_newDeckBtn.onClick.RemoveListener(NewDeckFunctionality);
            GlobalActions.OnDeckChanged -= ListenToChangeInDeckStatus;
            GlobalActions.OnNotSavingDeck -= ResolveUnsavedDeck;
        }

        private void ResolveUnsavedDeck(bool revertDeck)
        {
            if (!revertDeck)
            {
                if (notSavedCoroutine != null) StopCoroutine(notSavedCoroutine);
                resolveNotSaved = false;
                return;
            }

            resolveNotSaved = true;
        }


        private void SaveButtonFunctionality()
        {
            MainMenu_DeckManager.DeckSaved();
            MainMenu_DeckSaved.DeckChangedStatus = EnumDeckStatus.NotChanged;
        }

        private void NewDeckFunctionality()
        {
            notSavedCoroutine = StartCoroutine(StartCheckingToCreateDeck());
        }

        private IEnumerator StartCheckingToCreateDeck()
        {
            if (MainMenu_DeckSaved.DeckChangedStatus == EnumDeckStatus.Changed)
            {
                var warningComp = GetComponent<UI_DeckScrollBar>();
                if (warningComp == null)
                {
                    Debug.LogWarning("Find out why you don't have a Deck scroll bar component on here.");
                    yield return null;
                }
                warningComp.TriggerDeckNotSavedYetWarning();
                while (!resolveNotSaved) yield return null;
                resolveNotSaved = false;
                m_currentDeckManager.CreateADeck();
            }

            resolveNotSaved = false;
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
    }

}

