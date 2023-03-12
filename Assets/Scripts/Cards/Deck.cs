using System.Collections;
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

        public Deck() 
        {
            cards = new List<Card>();
        }

        public void AddCard(Card _card)
        {
            cards.Add(_card);
        }
        public void SetDeckName(string newName)
        {
            _deckName = newName;
        }

        public void CloneDeck(Deck deckToClone)
        {
            Debug.LogWarning(deckToClone.DeckName + " huh?");
            _deckName = deckToClone.DeckName;

            var _otherCards = deckToClone.Cards;

            if (_otherCards == null) return;

            for(int i = 0; i < _otherCards.Count; i++)
            {
                var _newCard = new Card();
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
    }

}