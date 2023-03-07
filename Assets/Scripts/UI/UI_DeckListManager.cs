using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class UI_DeckListManager : UI_ScrollView
    {
        private void OnEnable()
        {
            GlobalActions.OnPressCardButton += AddCardToDeckList;    
        }

        private void OnDisable()
        {
            GlobalActions.OnPressCardButton -= AddCardToDeckList;
        }


        private void AddCardToDeckList(Card card)
        {
            var _newBtn = Instantiate(m_buttonPrefab, m_content);
            var _btnComp = _newBtn.GetComponent<UI_DeckListButton>();
            if(_btnComp == null)
            {
                Debug.LogWarning("Your button does not have the correct component on it.");
                return;
            }

            _btnComp.InitializeButton(card);

        }
    }

}
