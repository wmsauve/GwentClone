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

    private List<Card> _highestPowerCards = new List<Card>();
    public List<Card> HighestPowerCard { get { return _highestPowerCards; } }

    public void CheckForNewHighestCard()
    {
        int _highestValue = 0;
        List<Card> _placeholder = new List<Card>();
        if (_highestPowerCards != null && _highestPowerCards.Count > 0)
        {
            _highestValue = _highestPowerCards[0].cardPower;
            _placeholder = _highestPowerCards;
        }

        foreach(Card card in _cardsInFront)
        {
            if (card.cardPower > _highestValue)
            {
                _highestValue = card.cardPower;
                _placeholder = new List<Card>();
                _placeholder.Add(card);
            }

            else if(card.cardPower == _highestValue)
            {
                _placeholder.Add(card);
            }
        }

        foreach (Card card in _cardsInRanged)
        {
            if (card.cardPower > _highestValue)
            {
                _highestValue = card.cardPower;
                _placeholder = new List<Card>();
                _placeholder.Add(card);
            }

            else if (card.cardPower == _highestValue)
            {
                _placeholder.Add(card);
            }
        }

        foreach (Card card in _cardsInSiege)
        {
            if (card.cardPower > _highestValue)
            {
                _highestValue = card.cardPower;
                _placeholder = new List<Card>();
                _placeholder.Add(card);
            }

            else if (card.cardPower == _highestValue)
            {
                _placeholder.Add(card);
            }
        }

        if(_placeholder != null)
        {
            _highestPowerCards = _placeholder;
        }
    }

    public void DestroyCardsOfPowerInPlay()
    {
        foreach(Card card in _highestPowerCards)
        {
            List<Card> zone = null;
            if (card.unitPlacement == EnumUnitPlacement.Frontline) zone = _cardsInFront;
            else if (card.unitPlacement == EnumUnitPlacement.Ranged) zone = _cardsInRanged;
            else if (card.unitPlacement == EnumUnitPlacement.Siege) zone = _cardsInSiege;

            if(zone == null)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Didn't get zone to remove card.");
                return;
            }

             var success = zone.Remove(card);
            if (!success) GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "The removed card should be here.");
        }
    }
}
