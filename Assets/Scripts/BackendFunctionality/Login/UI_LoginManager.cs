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

        [Header("Text Related")]
        [SerializeField] private TMP_InputField m_username = null;
        [SerializeField] private TMP_InputField m_password = null;
        [SerializeField] private TextMeshProUGUI m_userFeedbackText = null;

        private string fullAPI;
        private EnumAPIType whichAPICall;
        private APIManager manager;
        private Coroutine ongoingCoroutine;

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

            if (m_password == null || m_username == null || m_userFeedbackText == null)
            {
                Debug.LogWarning("Add text fields here.");
                return;
            }

            if(m_return.transform.parent != null) m_return.transform.parent.gameObject.SetActive(false);
            else m_return.gameObject.SetActive(false);

            m_loginButtons.SetActive(true);
            m_fillInFields.SetActive(false);
            m_userFeedbackText.gameObject.SetActive(false);

            manager = APIManager.Instance;
            if(manager == null)
            {
                Debug.LogWarning("Did you set a master instance of this Monobehavior?");
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
            m_return.onClick.AddListener(ReturnFunctionality);
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
            m_return.onClick.RemoveListener(ReturnFunctionality);
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

        private void ReturnFunctionality()
        {
            m_fillInFields.SetActive(false);
            m_loginButtons.SetActive(true);
            if (m_return.transform.parent != null) m_return.transform.parent.gameObject.SetActive(false);
            else m_return.gameObject.SetActive(false);
            m_userFeedbackText.gameObject.SetActive(false);
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

        public void OnSuccessfulAPICall(EnumAPIType type)
        {
            if (type != whichAPICall) return;

            if (ongoingCoroutine != null) StopCoroutine(ongoingCoroutine);
            m_return.interactable = true;
            m_accept.interactable = true;

            switch (whichAPICall)
            {
                case EnumAPIType.login:

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

