using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GwentClone
{
    public class UI_FillCardMenu : UI_ScrollView
    {
        [Header("All Available Cards Related")]
        [SerializeField] private Card[] m_allCards = null;

        protected override void InitializeThisUIComp()
        {
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

