using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class UI_DeckListManager : UI_ScrollView
    {
        private List<UI_DeckListButton> _addedButtons = new List<UI_DeckListButton>();


        protected override void Awake()
        {
            //I'm going to dynamically set the content here.
            if(m_buttonPrefab == null)
            {
                Debug.LogWarning("You didn't add a prefab to instantiate to this scrollview component.");
            }
        }

        public void SetCurrentContent(Transform newContent)
        {
            m_content = newContent;
            _addedButtons = new List<UI_DeckListButton>();

            if(m_content.childCount > 0)
            {
                for(int i = 0; i < m_content.childCount; i++)
                {
                    var _comp = m_content.transform.GetChild(i).GetComponentsInChildren<UI_DeckListButton>();
                    if(_comp == null)
                    {
                        Debug.LogWarning("Find out where your button component went.");
                        continue;
                    }
                    else if(_comp.Length > 1)
                    {
                        Debug.LogWarning("Find out why you have more than one of these components on the decklist button");
                        continue;
                    }

                    _addedButtons.Add(_comp[0]);
                }
            }
        }

        protected override void OnEnable()
        {
            GlobalActions.OnPressCardButton += AddCardToDeckList;
            GlobalActions.OnNotSavingDeck += SetOldDeckListButtons;
        }

        protected override void OnDisable()
        {
            GlobalActions.OnPressCardButton -= AddCardToDeckList;
            GlobalActions.OnNotSavingDeck -= SetOldDeckListButtons;
        }

        private void SetOldDeckListButtons(bool revertDeckButtons)
        {

        }


        private void AddCardToDeckList(Card card)
        {
            if(_addedButtons == null)
            {
                Debug.LogWarning("Find out why you don't have a list of buttons here.");
                return;
            }
            if (m_content == null)
            {
                Debug.LogWarning("Find out why you don't have a content here.");
                return;
            }

            var _isDuplicate = CheckDuplicateCard(card);
            if (_isDuplicate) return;

            var _newBtn = Instantiate(m_buttonPrefab, m_content);
            var _btnComp = _newBtn.GetComponent<UI_DeckListButton>();
            if(_btnComp == null)
            {
                Debug.LogWarning("Your button does not have the correct component on it.");
                return;
            }

            _addedButtons.Add(_btnComp);

            _btnComp.InitializeButton(card, this);

        }

        public IEnumerator RemoveFromCurrentButtons(Card card, Action<bool> callback)
        {
            UI_DeckListButton buttonToRemove = null;

            foreach(UI_DeckListButton buttons in _addedButtons)
            {
                if(buttons.CardData.id == card.id)
                {
                    buttonToRemove = buttons;
                    break;
                }
            }

            if(buttonToRemove == null) callback(false);
            else
            {
                _addedButtons.Remove(buttonToRemove);
                callback(true);
            }

            
            yield return null;
        }

        private bool CheckDuplicateCard(Card newCard)
        {
            foreach(UI_DeckListButton buttons in _addedButtons)
            {
                if (buttons.CardData.id == newCard.id)
                {
                    buttons.IncrementCardNumber();
                    return true;
                }
            }

            return false;
        }
    }

}
