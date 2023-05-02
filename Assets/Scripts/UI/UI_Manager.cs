using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    [Header("Components Related")]
    [SerializeField] private GameObject m_canvas = null;
    [SerializeField] private GameObject m_mainMenu = null;
    [SerializeField] private GameObject m_cardMenu = null;

    [Header("Version Check Related")]
    [SerializeField] private bool m_checkVersion = false;
    [SerializeField] private string m_fileVersionURI;
    [SerializeField] private GameObject m_failedVersionPrefab = null;
    private Coroutine m_versionCoroutine;

    [Header("Login Related")]
    [SerializeField] private bool m_login = false;
    [SerializeField] private GameObject m_loginMenu = null;
    [SerializeField] private GameObject m_startGameButtons = null;

    private void Start()
    {
        if(m_failedVersionPrefab == null)
        {
            Debug.LogWarning("You didn't place a version check prefab. You need this since the game is multiplayer.");
            return;
        }

#if UNITY_EDITOR
        if (!m_checkVersion) BeginPlayingGame();
        else m_versionCoroutine = StartCoroutine(CheckGameVersion());
#elif UNITY_STANDALONE_WIN
        m_versionCoroutine = StartCoroutine(CheckGameVersion());
#endif
    }

    private IEnumerator CheckGameVersion()
    {
        var _buildVer = Application.version;
        UnityWebRequest www = UnityWebRequest.Get(m_fileVersionURI);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string downloadedContents = www.downloadHandler.text;
            if (_buildVer.Equals(downloadedContents)) BeginPlayingGame();
            else
            {
                var _failedMessage = Instantiate(m_failedVersionPrefab, m_canvas.transform);
                var btn = _failedMessage.GetComponentInChildren<Button>();
                if (btn == null) Debug.LogWarning("Why is there no button here for the failed message?");
                else
                {
                    var buttonComp = _failedMessage.GetComponentInChildren<UI_OnButtonHover>();
                    if (buttonComp == null) Debug.LogWarning("Why is there no hover button here?");
                    else
                    {
                        buttonComp.InitializeThisUIComp();
                        btn.onClick.AddListener(CloseApplication);
                    }
                        
                }
            }
        }
        else Debug.LogError("Failed to download file: " + www.error);
    }

    private void BeginPlayingGame()
    {
        if (m_canvas == null)
        {
            Debug.LogWarning("You didn't place the canvas in the manager.");
            return;
        }

        if (m_mainMenu == null)
        {
            Debug.LogWarning("You didn't place the main menu screen in the manager.");
            return;
        }

        if(m_cardMenu == null)
        {
            Debug.LogWarning("You didn't place the card menu screen in the manager.");
            return;
        }

        if(m_loginMenu == null || m_startGameButtons == null)
        {
            Debug.LogWarning("You didn't place the main menu button objects in the manager.");
            return;
        }

        Debug.Log("Main Initialization: Set all active to ensure Initialization scripts are run");
        MakeSureElementsEnabled(m_canvas);

        Debug.Log("Main Initialization: Initialize all UI elements");
        GlobalActions.OnInitializeAllUI?.Invoke();

        Debug.Log("Main Initialization: Run checks for enabling or disabling overarching UI");

        m_cardMenu.SetActive(false);

#if UNITY_EDITOR
        if (m_login)
        {
            m_loginMenu.SetActive(true);
            m_startGameButtons.SetActive(false);
        }
        else
        {
            m_loginMenu.SetActive(false);
            m_startGameButtons.SetActive(true);
        }
#elif UNITY_STANDALONE_WIN
            m_loginMenu.SetActive(true);
            m_startGameButtons.SetActive(false);
#endif
    }


    public void OnCardSelectScreen()
    {
        m_mainMenu.SetActive(false);
        m_cardMenu.SetActive(true);
    }

    public void CloseApplication()
    {
        if(m_versionCoroutine != null)
        {
            StopCoroutine(m_versionCoroutine);
            m_versionCoroutine = null;
        }
        Application.Quit();
    }

    public void StartGame()
    {
    }

    private void MakeSureElementsEnabled(GameObject setActive)
    {
        if (!setActive.activeSelf)
        {
            setActive.SetActive(true);
        }

        if(setActive.transform.childCount == 0)
        {
            return;
        }

        for(int i = 0; i < setActive.transform.childCount; i++)
        {
            MakeSureElementsEnabled(setActive.transform.GetChild(i).gameObject);
        }


    }

}
