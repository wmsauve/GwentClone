using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GwentPlayer 
{
    private Deck m_playerDeck = null;
    public Deck Deck { get { return m_playerDeck; } set { m_playerDeck = value; } }

    private string m_playerName;
    public string Username { get { return m_playerName; } set { m_playerName = value; } }

    public GwentPlayer() 
    {
        m_playerName = "placeholder";
    }
    public GwentPlayer(string username, Deck deck)
    {
        m_playerDeck = deck;
        m_playerName = username;
    }
}
