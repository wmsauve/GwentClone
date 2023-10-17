using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class S_DeckManagers : NetworkBehaviour
{
    [Header("TestRelated")]
    [SerializeField] private List<GwentDeck> m_testDecks = null;
    [SerializeField] private GameObject m_cardRepoPrefab = null;

    private List<GwentPlayer> _gwentPlayers = new List<GwentPlayer>();
    public List<GwentPlayer> GwentPlayers { get { return _gwentPlayers; } }

    private CardRepository _cardRepo = null;
    public CardRepository CardRepo { get { return _cardRepo; } }

    public void RunCardRepoCheck()
    {
        if (_cardRepo != null) return;

        if (m_cardRepoPrefab == null)
        {
            Debug.LogWarning("If you are trying to test, you won't spawn a card repo.");
            return;
        }

        _cardRepo = GeneralPurposeFunctions.GetComponentFromScene<CardRepository>(m_cardRepoPrefab);
    }

    public void AddNewGwentPlayer(string username, ulong id)
    {
        var _newPlayer = new GwentPlayer(username, new Deck(), id);

        if (m_testDecks.Count < 1)
        {
            Debug.LogWarning("You don't have any test decks.");
            return;
        }

        GwentDeck _testDeck = GetTestDeck();

        _newPlayer.Deck.SetDeckLeader(_testDeck.m_deckLeader);
        foreach(Card card in _testDeck.m_card)
        {
            _newPlayer.Deck.AddCard(card);
        }

        _gwentPlayers.Add(_newPlayer);
    }

    private GwentDeck GetTestDeck()
    {
        var _rand = Random.Range(0, m_testDecks.Count);
        return m_testDecks[_rand];
    }
}
