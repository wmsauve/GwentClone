using System.Collections;
using System.Collections.Generic;

public class S_GameZones
{
    private List<Card> _cardsInFront = new List<Card>();
    private List<Card> _cardsInRanged = new List<Card>();
    private List<Card> _cardsInSiege = new List<Card>();

    public List<Card> CardsInFront { get { return _cardsInFront; } }
    public List<Card> CardsInRanged { get { return _cardsInRanged; } }
    public List<Card> CardsInSiege { get { return _cardsInSiege; } }

}
