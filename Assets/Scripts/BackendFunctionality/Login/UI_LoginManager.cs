using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GwentClone;
using UnityEngine.UI;
using TMPro;

namespace BackendFunctionality.Login
{
    public class UI_LoginManager : UI_InitializeFromManager
    {
        [Header("Main Panels")]
        [SerializeField] private GameObject m_loginButtons = null;
        [SerializeField] private GameObject m_fillInFields = null;

        [Header("Buttons")]
        [SerializeField] private Button m_login = null;
        [SerializeField] private Button m_signUp = null;
        [SerializeField] private Button m_return = null;
        [SerializeField] private Button m_accept = null;

        [Header("Text Fields")]
        [SerializeField] private TMP_InputField m_username = null;
        [SerializeField] private TMP_InputField m_password = null;

        protected override void InitializeThisUIComp()
        {

            if (m_loginButtons == null || m_fillInFields == null)
            {
                Debug.LogWarning("Add login objects here.");
                return;
            }

            if (m_login == null || m_signUp == null || m_return == null || m_accept == null)
            {
                Debug.LogWarning("You didn't add any buttons here.");
                return;
            }

            if (m_password == null || m_username == null)
            {
                Debug.LogWarning("Add text fields here.");
                return;
            }

            m_loginButtons.SetActive(true);
            m_fillInFields.SetActive(false);
            m_return.gameObject.SetActive(false);


        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_login == null || m_signUp == null || m_return == null || m_accept == null)
            {
                Debug.LogWarning("You didn't add any buttons here.");
                return;
            }

            m_login.onClick.AddListener(UserIsLoggingIn);
            m_signUp.onClick.AddListener(UserIsSigningUp);
            m_return.onClick.AddListener(ReturnFunctionality);
            m_accept.onClick.AddListener(RequestInfoFromServer);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (m_login == null || m_signUp == null || m_return == null || m_accept == null)
            {
                Debug.LogWarning("You didn't add any buttons here.");
                return;
            }

            m_login.onClick.RemoveListener(UserIsLoggingIn);
            m_signUp.onClick.RemoveListener(UserIsSigningUp);
            m_return.onClick.RemoveListener(ReturnFunctionality);
            m_accept.onClick.RemoveListener(RequestInfoFromServer);
        }

        private void UserIsLoggingIn()
        {

        }

        private void UserIsSigningUp()
        {

        }

        private void ReturnFunctionality()
        {

        }

        private void RequestInfoFromServer()
        {

        }
    }
}

