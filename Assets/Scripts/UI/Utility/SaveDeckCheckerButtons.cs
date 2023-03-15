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
        }

        private void OnDisable()
        {
            if (m_acceptButton == null || m_cancelButton == null) return;
            m_cancelButton.onClick.RemoveListener(CancelButtonFunctionality);
        }

        private void OnDestroy()
        {
            if (m_acceptButton == null || m_cancelButton == null) return;
            m_cancelButton.onClick.RemoveListener(CancelButtonFunctionality);
        }

        private void CancelButtonFunctionality()
        {
            Destroy(gameObject);
        }


    }

}

