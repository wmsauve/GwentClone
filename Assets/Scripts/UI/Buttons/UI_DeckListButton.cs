using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace GwentClone
{
    public class UI_DeckListButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Individual Component Related")]
        [SerializeField] private TextMeshProUGUI cardName = null;
        [SerializeField] private Image borderHighlight = null;
        [SerializeField] private Button myBtnComp = null;

        private Card myData = null;

        public void InitializeButton(Card card)
        {
            if(cardName == null || borderHighlight == null)
            {
                Debug.LogWarning("Your button does not have the correct component on it.");
                return;
            }

            myData = card;

            cardName.text = myData.id;
            if (myData.isHero)
            {
                borderHighlight.color = Color.yellow;
            }
            else
            {
                borderHighlight.color = new Color(0f,0f,0f,0f);
            }
        }

        private void OnEnable()
        {
            if(myBtnComp == null)
            {
                Debug.LogWarning("This should have a button component.");
                return;
            }

            myBtnComp.onClick.AddListener(RemoveCardFromDeck);
        }

        private void OnDisable()
        {
            myBtnComp.onClick.RemoveListener(RemoveCardFromDeck);
        }

        private void RemoveCardFromDeck()
        {
            Debug.LogWarning(myData.id + " This should remove the card for the deck.");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(myData == null)
            {
                Debug.LogWarning("Find out why this button doesn't have the reference to its own card.");
                return;
            }
            Debug.LogWarning(myData.id + " going in card.");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (myData == null)
            {
                Debug.LogWarning("Find out why this button doesn't have the reference to its own card.");
                return;
            }
            Debug.LogWarning(myData.id + " leaving card.");
        }
    }

}
