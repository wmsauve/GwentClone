using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GwentClone
{
    public class UI_HoverTooltip : MonoBehaviour
    {
        [SerializeField] private GameObject m_leaderUI = null;
        [SerializeField] private GameObject m_cardUI = null;
        [SerializeField] private Image m_objectSprite = null;

        private RectTransform canvasRect = null;
        private RectTransform mainRect = null;

        void Start()
        {
            if(m_leaderUI == null || m_cardUI == null || m_objectSprite == null)
            {
                Debug.LogWarning("Your tooltip doesnt have the references for the different UIs");
                return;
            }

            mainRect = GetComponent<RectTransform>();
            if(mainRect == null)
            {
                Debug.LogWarning("Add Rect Transform to this.");
                return;
            }

            var _canvas = GameObject.FindGameObjectWithTag(GlobalConstantValues.TAG_MAINCANVAS);
            if (_canvas == null)
            {
                Debug.LogWarning("Did you forget to add a MainUICanvas tag? Do you have a Canvas in your scene?");
                return;
            }

            canvasRect = _canvas.GetComponent<RectTransform>();

        }

        void Update()
        {
            if (canvasRect == null || mainRect == null) return;
            Vector2 anchoredPos = Input.mousePosition / canvasRect.localScale.x;

            if(anchoredPos.x + mainRect.rect.width > canvasRect.rect.width)
            {
                anchoredPos.x = canvasRect.rect.width - mainRect.rect.width;
            }
            if (anchoredPos.y + mainRect.rect.height > canvasRect.rect.height)
            {
                anchoredPos.y = canvasRect.rect.height - mainRect.rect.height;
            }

            mainRect.anchoredPosition = anchoredPos;
        }

        public void PassInfoToTooltip<T>(T info)
        {
            if(typeof(T) == typeof(Card))
            {
                var cardInfo = info as Card;
                m_objectSprite.sprite = cardInfo.cardImage;

            }
            else if (typeof(T) == typeof(Leader))
            {
                var leaderInfo = info as Leader;
                m_objectSprite.sprite = leaderInfo.cardImage;
            }
            else
            {
                Debug.Log("Not a type this method cares about.");
                return;
            }
        }
    }

}
