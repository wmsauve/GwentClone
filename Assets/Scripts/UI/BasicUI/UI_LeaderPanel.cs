using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone.UI
{
    public class UI_LeaderPanel : UI_ScrollView
    {
        [Header("Specific Objects Related")]
        [SerializeField] private GameObject m_mainPanel = null;

        [Header("Leaders Related")]
        [SerializeField] private List<Leader> m_leaders = new List<Leader>();

        protected override void InitializeThisUIComp()
        {
            if(m_mainPanel == null)
            {
                Debug.LogWarning("You didn't add the correct objects or components to initialize this component.");
                return;
            }

            if (m_leaders == null) return;
            if (m_leaders.Count == 0)
            {
                Debug.LogWarning("Add heroes here.");
                return;
            }

            AddLeaderButtons();


            m_mainPanel.SetActive(false);
        }

        private void AddLeaderButtons()
        {
            foreach(Leader leader in m_leaders)
            {
                var _newButton = Instantiate(m_buttonPrefab, m_content);
                var _buttonComp = _newButton.GetComponentInChildren<UI_LeaderButton>();
                if(_buttonComp == null)
                {
                    Debug.LogWarning("Find out why you don't have the button comp on your Leader button.");
                    return;
                }

                _buttonComp.InitializeButton(leader);
            }
        }
    }

}

