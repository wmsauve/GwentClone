using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GwentClone
{
    public class UI_DeckListButton : MonoBehaviour
    {
        [Header("Individual Component Related")]
        [SerializeField] private TextMeshProUGUI cardName = null;
        [SerializeField] private Image borderHighlight = null;

        public void InitializeButton(Card card)
        {
            if(cardName == null || borderHighlight == null)
            {
                Debug.LogWarning("Your button does not have the correct component on it.");
                return;
            }

            cardName.text = card.id;
            if (card.isHero)
            {
                borderHighlight.color = Color.yellow;
            }
            else
            {
                borderHighlight.color = new Color(0f,0f,0f,0f);
            }
        }
    }

}
