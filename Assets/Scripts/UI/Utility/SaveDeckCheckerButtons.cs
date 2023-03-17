using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GwentClone
{
    public class SaveDeckCheckerButtons : MonoBehaviour
    {
        [SerializeField] private Button m_acceptButton = null;
        [SerializeField] private Button m_cancelButton = null;


        // Start is called before the first frame update
        void Start()
        {
            if(m_acceptButton == null || m_cancelButton == null)
            {
                Debug.LogWarning("Check your prefab and set your buttons");
            }
        }

        private void OnEnable()
        {
            if (m_acceptButton == null || m_cancelButton == null) return;

            m_cancelButton.onClick.AddListener(CancelButtonFunctionality);
            m_acceptButton.onClick.AddListener(SkipSavingFunctionality);
        }

        private void OnDisable()
        {
            if (m_acceptButton == null || m_cancelButton == null) return;
            m_cancelButton.onClick.RemoveListener(CancelButtonFunctionality);
            m_acceptButton.onClick.RemoveListener(SkipSavingFunctionality);
        }

        private void OnDestroy()
        {
            if (m_acceptButton == null || m_cancelButton == null) return;
            m_cancelButton.onClick.RemoveListener(CancelButtonFunctionality);
            m_acceptButton.onClick.RemoveListener(SkipSavingFunctionality);
        }

        private void CancelButtonFunctionality()
        {

            GlobalActions.OnNotSavingDeck?.Invoke(false);
            Destroy(gameObject);
        }

        private void SkipSavingFunctionality()
        {
            var success = MainMenu_DeckManager.RevertCurrentDeckToClone();

            if (!success) return;

            GlobalActions.OnNotSavingDeck?.Invoke(true);
            Destroy(gameObject);
        }

    }

}

