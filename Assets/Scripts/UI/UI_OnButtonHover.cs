using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace GwentClone
{
    public class UI_OnButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Objects Related")]
        [SerializeField] private GameObject m_borderElement = null;

        [Header("Parameters Related")]
        [SerializeField] private string m_text = "";

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_borderElement == null) return;

            var _borderImage = m_borderElement.GetComponent<Image>();
            if (_borderImage == null) return;
            _borderImage.enabled = true;

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (m_borderElement == null) return;

            var _borderImage = m_borderElement.GetComponent<Image>();
            if (_borderImage == null) return;
            _borderImage.enabled = false;

        }

        private void OnEnable()
        {
            UI_Manager.initializeAllUI += InitializeThisButton;
        }

        private void OnDisable()
        {
            UI_Manager.initializeAllUI -= InitializeThisButton;
        }

        private void InitializeThisButton()
        {
            var _text = transform.GetComponentInChildren<TextMeshProUGUI>();
            if (_text == null) return;
            if (m_borderElement == null) return;
            _text.text = m_text;
            var _borderImage = m_borderElement.GetComponent<Image>();
            if (_borderImage == null) return;
            _borderImage.enabled = false;

        }

    }

}

