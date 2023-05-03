using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class S_DeckManagers : NetworkBehaviour
{
    [Header("TestRelated")]
    public Leader m_testLeader1;
    public Leader m_testLeader2;
    public List<Card> m_testDeckCards1 = new List<Card>();
    public List<Card> m_testDeckCards2 = new List<Card>();

    private GwentPlayer _firstPlayer;
    private GwentPlayer _secondPlayer;

    public void StorePlayersDecks()
    {

    }

}
