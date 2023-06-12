public class GwentPlayer 
{
    private Deck m_playerDeck = null;
    public Deck Deck { get { return m_playerDeck; } set { m_playerDeck = value; } }

    private string m_playerName;
    public string Username { get { return m_playerName; } set { m_playerName = value; } }

    private ulong _uniqueID;
    public ulong ID { get { return _uniqueID; } }

    public GwentPlayer(ulong id) 
    {
        m_playerName = "placeholder";
        _uniqueID = id;
    }
    public GwentPlayer(string username, Deck deck, ulong id)
    {
        m_playerDeck = deck;
        m_playerName = username;
        _uniqueID = id;
    }
}
