using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GwentClone;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

namespace BackendFunctionality.Login
{
    public class UI_LoginManager : UI_InitializeFromManager
    {
        [Header("Main Panels")]
        [SerializeField] private GameObject m_loginButtons = null;
        [SerializeField] private GameObject m_fillInFields = null;
        [SerializeField] private GameObject m_toEnableOnLogin = null;

        [Header("Buttons")]
        [SerializeField] private Button m_login = null;
        [SerializeField] private Button m_signUp = null;
        [SerializeField] private Button m_return = null;
        [SerializeField] private Button m_accept = null;

        [Header("Text Related")]
        [SerializeField] private TMP_InputField m_username = null;
        [SerializeField] private TMP_InputField m_password = null;
        [SerializeField] private Selectable[] m_inputFields = null;
        [SerializeField] private TextMeshProUGUI m_userFeedbackText = null;

        private string fullAPI;
        private EnumAPIType whichAPICall;
        private APIManager manager;
        private Coroutine ongoingCoroutine;

        protected override void InitializeThisUIComp()
        {

            if (m_loginButtons == null || m_fillInFields == null || m_toEnableOnLogin == null)
            {
                Debug.LogWarning("Add login objects here.");
                return;
            }

            if (m_login == null || m_signUp == null || m_return == null || m_accept == null)
            {
                Debug.LogWarning("You didn't add any buttons here.");
                return;
            }

            if (m_password == null || m_username == null || m_userFeedbackText == null)
            {
                Debug.LogWarning("Add text fields here.");
                return;
            }

            if (m_inputFields == null)
            {
                Debug.LogWarning("Add your input fields here to be able to tab through it.");
                return;
            }

            if(m_return.transform.parent != null) m_return.transform.parent.gameObject.SetActive(false);
            else m_return.gameObject.SetActive(false);

            m_loginButtons.SetActive(true);
            m_fillInFields.SetActive(false);
            m_userFeedbackText.gameObject.SetActive(false);

            char passwordMask = '*';
            m_password.contentType = TMP_InputField.ContentType.Password;
            m_password.asteriskChar = passwordMask;

            manager = APIManager.Instance;
            if(manager == null)
            {
                Debug.LogWarning("Did you set a master instance of this Monobehavior?");
            }
        }

        private void Update()
        {
            if (m_accept.isActiveAndEnabled && m_accept.interactable)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    RequestInfoFromServer();
                }
            }

            if(m_inputFields.Length > 0)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Selectable _next = FunctionLibrary.CycleBetweenSelectables(m_inputFields);
                    if (_next != null) _next.Select();
                }
            }

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
            m_return.onClick.AddListener(delegate { ReturnFunctionality(""); });
            m_accept.onClick.AddListener(RequestInfoFromServer);

            GlobalBackendActions.OnFinishedAPICall += OnSuccessfulAPICall;
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
            m_return.onClick.RemoveListener(delegate { ReturnFunctionality(""); });
            m_accept.onClick.RemoveListener(RequestInfoFromServer);

            GlobalBackendActions.OnFinishedAPICall -= OnSuccessfulAPICall;

            if (ongoingCoroutine != null) StopCoroutine(ongoingCoroutine);
        }

        private void UserIsLoggingIn()
        {
            fullAPI = manager.API_URL + manager.API_ENDPOINT_LOGIN;
            m_fillInFields.SetActive(true);
            m_loginButtons.SetActive(false);
            if (m_return.transform.parent != null) m_return.transform.parent.gameObject.SetActive(true);
            else m_return.gameObject.SetActive(true);
            m_userFeedbackText.gameObject.SetActive(true);
            m_userFeedbackText.text = GlobalConstantValues.MESSAGE_LOGINMESSAGE;

            whichAPICall = EnumAPIType.login;
        }

        private void UserIsSigningUp()
        {
            fullAPI = manager.API_URL + manager.API_ENDPOINT_SIGNUP;
            m_fillInFields.SetActive(true);
            m_loginButtons.SetActive(false);
            if (m_return.transform.parent != null) m_return.transform.parent.gameObject.SetActive(true);
            else m_return.gameObject.SetActive(true);
            m_userFeedbackText.gameObject.SetActive(true);
            m_userFeedbackText.text = GlobalConstantValues.MESSAGE_SIGNUPMESSAGE;

            whichAPICall = EnumAPIType.signup;
        }

        private void ReturnFunctionality(string message)
        {
            m_fillInFields.SetActive(false);
            m_loginButtons.SetActive(true);
            if (m_return.transform.parent != null) m_return.transform.parent.gameObject.SetActive(false);
            else m_return.gameObject.SetActive(false);
            m_userFeedbackText.text = message;
            fullAPI = "";
        }

        private void RequestInfoFromServer()
        {
            if (ongoingCoroutine != null) return;

            m_return.interactable = false;
            m_accept.interactable = false;

            switch (whichAPICall)
            {
                case EnumAPIType.login:
                    UserData _user = new UserData();

                    _user.username = UseUsername();
                    _user.password = UsePassword();
                    if (_user.username == "" || _user.password == "") break;
                    ongoingCoroutine = StartCoroutine(APIManager.Instance.PostRequest(fullAPI, _user, whichAPICall));

                    break;
                case EnumAPIType.signup:
                    UserData _newUser = new UserData();

                    _newUser.username = UseUsername();
                    _newUser.password = UsePassword();
                    if (_newUser.username == "" || _newUser.password == "") break;
                    ongoingCoroutine = StartCoroutine(APIManager.Instance.PostRequest(fullAPI, _newUser, whichAPICall));
                    break;
            }
            
        }

        public void OnSuccessfulAPICall(EnumAPIType type, ResponseFromServer response)
        {
            if (type != whichAPICall) return;

            if (ongoingCoroutine != null)
            {
                StopCoroutine(ongoingCoroutine);
                ongoingCoroutine = null;
            }
            m_return.interactable = true;
            m_accept.interactable = true;

            m_username.text = "";
            m_password.text = "";

            switch (whichAPICall)
            {
                case EnumAPIType.login:
                    if(response.isSuccess)
                    {
                        m_toEnableOnLogin.SetActive(true);
                        gameObject.SetActive(false);
                        break;
                    }
                    m_userFeedbackText.text = response.message;
                    
                    break;
                case EnumAPIType.signup:
                    if (response.isSuccess)
                    {
                        ReturnFunctionality(response.message);
                        break;
                    }
                    m_userFeedbackText.text = response.message;
                    break;
            }
        }
        private string UseUsername()
        {
            if (m_username.gameObject.activeSelf)
            {
                if (m_username.text.Length > 20)
                {
                    m_userFeedbackText.text = GlobalConstantValues.MESSAGE_INPUTFIELDTOOLONG;
                    return "";
                }
            }
            return m_username.text;
        }

        private string UsePassword()
        {
            if (m_password.gameObject.activeSelf)
            {
                if (m_password.text.Length < 8)
                {
                    m_userFeedbackText.text = GlobalConstantValues.MESSAGE_SHORTPASSWORD;
                    return "";
                }
                else if (m_password.text.Length > 20)
                {
                    m_userFeedbackText.text = GlobalConstantValues.MESSAGE_INPUTFIELDTOOLONG;
                    return "";
                }
                
            }
            return m_password.text;
        }
    }
}

