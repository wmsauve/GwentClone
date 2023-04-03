using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class CardRepository : MonoBehaviour
    {
        [Header("All Available Cards Related")]
        [SerializeField] private Card[] m_allCards = null;
        public Card[] AllCards { get { return m_allCards; } }

        private Dictionary<string, Card> m_cardFetch = new Dictionary<string, Card>();

        private void Awake()
        {
            if(m_allCards == null)
            {
                return;
            }
            if (m_allCards.Length == 0)
            {
                Debug.LogWarning("You haven't placed any cards to play the game with.");
                return;
            }


            foreach(Card _card in m_allCards)
            {
                m_cardFetch.Add(_card.id, _card);
            }
        }

        public Card GetCard(string _cardID)
        {
            Card newCard;
            var success = m_cardFetch.TryGetValue(_cardID, out newCard);
            if (success)
            {
                return newCard;
            }
            else
            {
                Debug.LogWarning("Invalid card Id. Try again.");
                return null;
            }
        }
    }

}

