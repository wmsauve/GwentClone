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
        [SerializeField] private Leader[] m_allLeaders = null;
        public Leader[] AllLeaders { get { return m_allLeaders; } }

        private Dictionary<string, Card> m_cardFetch = new Dictionary<string, Card>();

        private Dictionary<string, Leader> m_leaderFetch = new Dictionary<string, Leader>();

        private void Awake()
        {
            if(m_allCards == null) return;
            if (m_allCards.Length == 0)
            {
                Debug.LogWarning("You haven't placed any cards to play the game with.");
                return;
            }

            if (m_allLeaders == null) return;
            if (m_allLeaders.Length == 0)
            {
                Debug.LogWarning("You haven't placed any leaders to play the game with.");
                return;
            }


            foreach (Card _card in m_allCards)
            {
                m_cardFetch.Add(_card.id, _card);
            }

            foreach(Leader _leader in m_allLeaders)
            {
                m_leaderFetch.Add(_leader.id, _leader);
            }
        }

        public Card GetCard(string _cardID)
        {
            Card newCard;
            var success = m_cardFetch.TryGetValue(_cardID, out newCard);
            if (success) return newCard;
            else
            {
                Debug.LogWarning("Invalid card Id. Try again.");
                return null;
            }
        }

        public Leader GetLeader(string _leaderID)
        {
            Leader newLeader;
            var success = m_leaderFetch.TryGetValue(_leaderID, out newLeader);
            if (success) return newLeader;
            else
            {
                Debug.LogWarning("Invalid leader ID. Try again.");
                return null;
            }
        }
    }

}

