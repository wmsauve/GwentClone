using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Test_NetworkManager : MonoBehaviour
{
    [SerializeField] private Button m_serverBtn = null;
    [SerializeField] private Button m_hostBtn = null;
    [SerializeField] private Button m_clientBtn = null;

    private void Awake()
    {
        if(m_serverBtn == null)
        {
            Debug.LogWarning("You can't create a server from this running instance by pressing this button.");
        }
        else
        {
            m_serverBtn.onClick.AddListener(() => {
                NetworkManager.Singleton.StartServer();
            });
        }

        if (m_hostBtn == null)
        {
            Debug.LogWarning("You can't create a host from this running instance by pressing this button.");
        }
        else
        {
            m_hostBtn.onClick.AddListener(() => {
                NetworkManager.Singleton.StartHost();
            });
        }

        if (m_clientBtn == null)
        {
            Debug.LogWarning("You can't create a client from this running instance by pressing this button.");
        }
        else
        {
            m_clientBtn.onClick.AddListener(() => {
                NetworkManager.Singleton.StartClient();
            });
        }
    }
}
