using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class S_DeckManagers : NetworkBehaviour
{
    [System.Serializable]
    private class TestDeckSetup
    {
        public Leader testLeader;
        public List<Card> testCards;
    }

    [Header("TestRelated")]
    [SerializeField] private List<TestDeckSetup> m_testDecks = null;

    private List<GwentPlayer> _gwentPlayers = new List<GwentPlayer>();
    public List<GwentPlayer> GwentPlayers { get { return _gwentPlayers; } }

    public void AddNewGwentPlayer(string username, ulong id)
    {
        var _newPlayer = new GwentPlayer(username, new Deck(), id);

        TestDeckSetup _testDeck = GetTestDeck();

        _newPlayer.Deck.SetDeckLeader(_testDeck.testLeader);
        foreach(Card card in _testDeck.testCards)
        {
            _newPlayer.Deck.AddCard(card);
        }

        _gwentPlayers.Add(_newPlayer);
    }

    private TestDeckSetup GetTestDeck()
    {
        var _rand = Random.Range(0, m_testDecks.Count);
        return m_testDecks[_rand];
    }

    private void Update()
    {
        if (IsServer)
        {
        }

        if (IsClient)
        {
        }
    }

}
