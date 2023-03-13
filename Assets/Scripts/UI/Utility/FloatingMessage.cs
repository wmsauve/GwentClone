using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GwentClone
{
    public class FloatingMessage : MonoBehaviour
    {
        [SerializeField] private GameObject m_floatingMessagePrefab = null;
        private GameObject _mainCanvas = null;

        private void Start()
        {
            if(m_floatingMessagePrefab == null)
            {
                Debug.LogWarning("You are not instantiating a floating message.");
            }
           
            _mainCanvas = GameObject.FindGameObjectWithTag(GlobalConstantValues.TAG_MAINCANVAS);
            if(_mainCanvas == null)
            {
                Debug.LogWarning("Did you forget to add a MainUICanvas tag? Do you have a Canvas in your scene?");
            }

        }

        private void OnEnable()
        {
            GlobalActions.OnDisplayFeedbackInUI += ShowMessage;
        }

        private void OnDisable()
        {
            GlobalActions.OnDisplayFeedbackInUI -= ShowMessage;
        }

        private void ShowMessage(string message)
        {
            if (m_floatingMessagePrefab == null || _mainCanvas == null) return;

            var newMessage = Instantiate(m_floatingMessagePrefab, _mainCanvas.transform);
            var textComp = newMessage.GetComponent<TextMeshProUGUI>();
            if (textComp == null) return;


            textComp.text = message;
            var animComp = newMessage.GetComponent<Anim_FloatingMessage>();
            if (animComp == null) return;
            animComp.BeginAnimation = true;
        }
    }

}

