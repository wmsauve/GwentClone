using System;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class Deck
    {
        private List<Card> cards;
        public List<Card> Cards { get { return cards; } }

        private string _deckName;
        public string DeckName { get { return _deckName; } }

        private string _deckUID;
        public string DeckUID { get { return _deckUID; } }

        private int _totalCards;
        public int TotalCards { get { return _totalCards; } }
        private int _numOfUnits;
        public int NumberOfUnits { get { return _numOfUnits; } }
        private int _specialCards;
        public int SpecialCards { get { return _specialCards; } }
        private int _totalStrength;
        public int TotalStrength { get { return _totalStrength; } }
        private int _heroCards;
        public int HeroCards { get { return _heroCards; } }

        public Deck() 
        {
            cards = new List<Card>();
            _deckUID = Guid.NewGuid().ToString();
        }

        public void AddCard(Card _card)
        {
            cards.Add(_card);

            RecalculateDeckInfo();
        }

        public void RemoveCard(Card _cardToRemove)
        {
            foreach(Card card in cards)
            {
                if(card.id == _cardToRemove.id)
                {
                    cards.Remove(card);
                    RecalculateDeckInfo();
                    break;
                }
            }
        }

        public void SetDeckName(string newName)
        {
            _deckName = newName;
        }

        public void CloneDeck(Deck deckToClone)
        {
            _deckName = deckToClone.DeckName;

            _deckUID = deckToClone._deckUID;

            var _otherCards = deckToClone.Cards;

            if (_otherCards == null) return;

            for(int i = 0; i < _otherCards.Count; i++)
            {
                var _newCard = ScriptableObject.CreateInstance<Card>();
                _newCard.cardImage = _otherCards[i].cardImage;
                _newCard.id = _otherCards[i].id;
                _newCard.unitType = _otherCards[i].unitType;
                _newCard.unitPlacement = _otherCards[i].unitPlacement;
                _newCard.factionType = _otherCards[i].factionType;
                _newCard.specialEffect = _otherCards[i].specialEffect;
                _newCard.isHero = _otherCards[i].isHero;
                _newCard.cardPower = _otherCards[i].cardPower;
                _newCard.cardType = _otherCards[i].cardType;
                _newCard.maxPerDeck = _otherCards[i].maxPerDeck;

                cards.Add(_newCard);
            }
        }

        private void RecalculateDeckInfo()
        {
            _totalCards = cards.Count;

            _heroCards = 0;
            _numOfUnits = 0;
            _specialCards = 0;
            _totalStrength = 0;

            foreach(Card card in cards)
            {
                if (card.isHero)
                {
                    _heroCards++;
                }

                if(card.cardType == EnumCardType.Unit)
                {
                    _numOfUnits++;
                }
                else
                {
                    _specialCards++;
                }

                _totalStrength += card.cardPower;

            }
        }
    }

}