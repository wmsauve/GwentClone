using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GwentClone
{
    public class UI_Manager : MonoBehaviour
    {
        [Header("Components Related")]
        [SerializeField] private GameObject m_canvas = null;
        [SerializeField] private GameObject m_mainMenu = null;
        [SerializeField] private GameObject m_cardMenu = null;
              

        [Header("Login Related")]
        [SerializeField] private bool m_testLogin = false;
        [SerializeField] private GameObject m_loginMenu = null;
        [SerializeField] private GameObject m_startGameButtons = null;

        void Start()
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
            if (m_testLogin)
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

}

