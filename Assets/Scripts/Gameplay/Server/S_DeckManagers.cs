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
    [SerializeField] private GameObject m_cardRepoPrefab = null;

    private List<GwentPlayer> _gwentPlayers = new List<GwentPlayer>();
    public List<GwentPlayer> GwentPlayers { get { return _gwentPlayers; } }

    private CardRepository _cardRepo = null;
    public CardRepository CardRepo { get { return _cardRepo; } }

    public void RunCardRepoCheck()
    {
        if (IsClient)
        {
            if (_cardRepo != null) return;

            if (m_cardRepoPrefab == null)
            {
                Debug.LogWarning("If you are trying to test, you won't spawn a card repo.");
                return;
            }

            var _cardRepos = FindObjectsOfType<CardRepository>();
            if (_cardRepos.Length == 0)
            {
                var _repoObj = Instantiate(m_cardRepoPrefab);
                _cardRepo = _repoObj.GetComponent<CardRepository>();
                return;
            }

            if(_cardRepos.Length > 1)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "You don't need 2 card repos.");
                return;
            }

            _cardRepo = _cardRepos[0];
        }
    }

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
}
