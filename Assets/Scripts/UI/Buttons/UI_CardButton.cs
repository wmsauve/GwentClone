using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GwentClone.UI
{
    public class UI_CardButton : UI_ToolTipButton
    {
        private Image m_image = null;
        private Card m_myData = null;

        private Button myBtnComp = null;

        public void InitializeCardButton(Card cardData)
        {
            m_image = GetComponent<Image>();
            if(m_image == null)
            {
                Debug.LogWarning("Your card button doesn't have an image.");
                return;
            }

            m_myData = cardData;

            m_image.sprite = m_myData.cardImage;
        }

        private void OnEnable()
        {
            myBtnComp = gameObject.GetComponent<Button>();
            if (myBtnComp == null)
            {
                Debug.LogWarning("This should have a button comp");
                return;
            }
            myBtnComp.onClick.AddListener(SendCardData);
        }

        private void OnDisable()
        {
            myBtnComp.onClick.RemoveListener(SendCardData);
        }

        private void SendCardData()
        {
            var addCard = MainMenu_DeckManager.AddCardToCurrentDeck(m_myData);

            if (!addCard) return;

            GlobalActions.OnPressCardButton?.Invoke(m_myData);
            
        }


        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            GlobalActions.OnHoveredUIButton?.Invoke(m_myData, null);
        }
    }

}
