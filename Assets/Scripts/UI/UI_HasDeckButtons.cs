using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GwentClone
{
    public class UI_HasDeckButtons : UI_InitializeFromManager
    {
        [SerializeField] private Button m_saveDeckBtn = null;
        [SerializeField] private Button m_newDeckBtn = null;



        protected override void InitializeThisUIComp()
        {
            if (m_saveDeckBtn == null || m_newDeckBtn == null)
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

        }

        private void NewDeckFunctionality()
        {

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
                default:
                    Debug.LogWarning("Check to see if the Enum is correct.");
                    break;
            }
        }
    }

}

