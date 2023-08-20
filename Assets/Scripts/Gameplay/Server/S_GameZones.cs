using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class S_GameZones
{
    public class GameZone
    {
        public List<Card> Cards = new List<Card>();
        public int TotalPower;

        public List<Card> HighestPowerCards = new List<Card>();
        public int HighestPowerCard;
        
        public void AddCardToZone(Card _newCard)
        {
            Cards.Add(_newCard);
            RunHighestCardCheck();
        }

        public void DestroyCardsOfPower(int power)
        {
            List<Card> savedCards = new List<Card>();
            foreach(Card _card in Cards)
            {
                if (_card.cardEffects.Contains(EnumCardEffects.Hero) || _card.cardPower != power) savedCards.Add(_card);
            }

            Cards = savedCards;
            RunHighestCardCheck();
        }

        private void RunHighestCardCheck()
        {
            if (Cards == null || Cards.Count == 0) return;

            TotalPower = 0;

            foreach(Card _card in Cards)
            {
                if (_card.cardEffects.Contains(EnumCardEffects.Hero)) continue;

                TotalPower += _card.cardPower;

                if (_card.cardPower > HighestPowerCard)
                {
                    HighestPowerCard = _card.cardPower;
                    HighestPowerCards = new List<Card>();
                    HighestPowerCards.Add(_card);
                }

                else if (_card.cardPower == HighestPowerCard) HighestPowerCards.Add(_card);
            }
        }
    }

    private GameZone _cardsInFront = new GameZone();
    private GameZone _cardsInRanged = new GameZone();
    private GameZone _cardsInSiege = new GameZone();

    public GameZone CardsInFront { get { return _cardsInFront; } }
    public GameZone CardsInRanged { get { return _cardsInRanged; } }
    public GameZone CardsInSiege { get { return _cardsInSiege; } }

    private List<Card> _highestPowerCards = new List<Card>();
    public List<Card> HighestPowerCard { get { return _highestPowerCards; } }

    private int _currentHighestPower;

    public void CheckForNewHighestCard()
    {
        _currentHighestPower = _cardsInFront.HighestPowerCard;
        if (_cardsInRanged.HighestPowerCard > _currentHighestPower) _currentHighestPower = _cardsInRanged.HighestPowerCard;
        if (_cardsInSiege.HighestPowerCard > _currentHighestPower) _currentHighestPower = _cardsInSiege.HighestPowerCard;

        //Store highest in a separate list
        _highestPowerCards.Clear();
        if (_cardsInFront.HighestPowerCard == _currentHighestPower
        && _cardsInFront.HighestPowerCards != null
        && _cardsInFront.HighestPowerCards.Count > 0) _highestPowerCards.AddRange(_cardsInFront.HighestPowerCards);
        if (_cardsInRanged.HighestPowerCard == _currentHighestPower
        && _cardsInRanged.HighestPowerCards != null
        && _cardsInRanged.HighestPowerCards.Count > 0) _highestPowerCards.AddRange(_cardsInRanged.HighestPowerCards);
        if (_cardsInSiege.HighestPowerCard == _currentHighestPower
        && _cardsInSiege.HighestPowerCards != null
        && _cardsInSiege.HighestPowerCards.Count > 0) _highestPowerCards.AddRange(_cardsInSiege.HighestPowerCards);
    }

    public bool DestroyCardsOfPowerInPlay()
    {
        _cardsInFront.DestroyCardsOfPower(_currentHighestPower);
        _cardsInRanged.DestroyCardsOfPower(_currentHighestPower);
        _cardsInSiege.DestroyCardsOfPower(_currentHighestPower);

        CheckForNewHighestCard();
        return true;
    }
}
