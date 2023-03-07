using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class UI_DeckScrollBar : UI_ScrollView
    {

        public void AddDeck()
        {
            var _newBtn = Instantiate(m_buttonPrefab, m_content);
            var _btnComp = _newBtn.GetComponent<UI_DeckButton>();
            if (_btnComp == null)
            {
                Debug.LogWarning("Your button doesn't have the main functionality component.");
                return;
            }
            _btnComp.InitializeDeckButton();
        }
    }

}
