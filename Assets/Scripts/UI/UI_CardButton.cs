using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GwentClone
{
    public class UI_CardButton : MonoBehaviour
    {
        private Image m_image = null;
        private Card m_myData = null;

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

            var _btnComp = gameObject.GetComponent<Button>();
            if(_btnComp == null)
            {
                Debug.LogWarning("This should have a button comp");
                return;
            }

            _btnComp.onClick.AddListener(SendCardData);
        }

        private void SendCardData()
        {
            GlobalActions.OnPressCardButton.Invoke(m_myData);
        }

    }

}
