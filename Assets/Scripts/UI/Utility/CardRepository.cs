using System.Collections.Generic;
using UnityEngine;

public class CardRepository : MonoBehaviour
{
    [System.Serializable]
    public struct CardsInDeck
    {
        public EnumFactionType DeckFaction;
        public Card[] AvailableCards;
    }

    [Header("All Available Cards Related")]
    [SerializeField] private List<CardsInDeck> m_allCards = new List<CardsInDeck>();
    public List<CardsInDeck> AllCards { get { return m_allCards; } }
    [SerializeField] private Leader[] m_allLeaders = null;
    public Leader[] AllLeaders { get { return m_allLeaders; } }

    private Dictionary<string, Card> m_cardFetch = new Dictionary<string, Card>();

    private Dictionary<string, Leader> m_leaderFetch = new Dictionary<string, Leader>();

    private void Awake()
    {
        if(m_allCards == null) return;
        if (m_allCards.Count == 0)
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


        foreach (CardsInDeck _deck in m_allCards)
        {
            foreach(Card _card in _deck.AvailableCards)
            {
                m_cardFetch.Add(_card.id, _card);
            }
        }

        foreach (Leader _leader in m_allLeaders)
        {
            m_leaderFetch.Add(_leader.id, _leader);
        }
    }

    public Card GetCard(string _cardID)
    {
        Card newCard;
        var success = m_cardFetch.TryGetValue(_cardID, out newCard);
        if (success)
        {
            //newCard.InitializeCard();
            return newCard;
        }
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

    public List<Leader> GetLeadersByFaction(EnumFactionType faction)
    {
        List<Leader> list = new List<Leader>();

        foreach(Leader leader in m_allLeaders)
        {
            if (leader.factionType == faction) list.Add(leader);
        }


        return list;
    }

    public void SetAsDoNotDestroy()
    {
        DontDestroyOnLoad(gameObject);
    }
}

