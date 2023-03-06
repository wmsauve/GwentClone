using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GwentClone
{
    public class UI_FillCardMenu : MonoBehaviour
    {

        [SerializeField] private Card[] m_allCards = null;
        [SerializeField] private Transform m_content = null;
        [SerializeField] private GameObject m_buttonPrefab = null;

        private void OnEnable()
        {
            GlobalActions.OnInitializeAllUI += InitializeCardList;
        }

        private void OnDisable()
        {
            GlobalActions.OnInitializeAllUI -= InitializeCardList;
        }

        private void InitializeCardList()
        {
            if (m_content == null)
            {
                Debug.LogWarning("You didn't put your content object in the Scroll View component.");
                return;
            }

            if(m_buttonPrefab == null)
            {
                Debug.LogWarning("You aren't going to instantiate any buttons.");
                return;
            }

            foreach(Card _card in m_allCards)
            {
                var _newButton = Instantiate(m_buttonPrefab, m_content);
                var buttonComp = _newButton.GetComponent<UI_CardButton>();
                if(buttonComp == null)
                {
                    Debug.LogWarning("Make sure your instantiated button has the right component on it.");
                    return;
                }

                buttonComp.InitializeCardButton(_card);
            }

        }
    }

}

