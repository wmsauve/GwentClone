using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GwentClone
{
    public class UI_Manager : MonoBehaviour
    {

        

        [SerializeField] private GameObject m_mainMenu = null;
        [SerializeField] private GameObject m_cardMenu = null;

        void Start()
        {


            if(m_mainMenu == null)
            {
                Debug.LogWarning("You didn't place the main menu screen in the manager.");
            }

            if(m_cardMenu == null)
            {
                Debug.LogWarning("You didn't place the card menu screen in the manager.");
            }

            if(!m_mainMenu.activeSelf) m_mainMenu.SetActive(true);
            if (!m_cardMenu.activeSelf) m_cardMenu.SetActive(true);

            Debug.Log("Main Initialization: Set all active to ensure Initialization scripts are run");
            MakeSureElementsEnabled(m_cardMenu);

            Debug.Log("Main Initialization: Initialize all UI elements");
            GlobalActions.OnInitializeAllUI?.Invoke();

            m_cardMenu.SetActive(false);


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

