using System;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class Deck
    {
        private bool isCurrentDeck;
        public bool IsCurrentDeck { get { return isCurrentDeck; } set { isCurrentDeck = value; } }

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

        private Leader deckLeader;
        public Leader DeckLeader { get { return deckLeader; } }


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

        public void SetDeckLeader(Leader leader)
        {
            if(deckLeader == null)
            {
                deckLeader = leader;
                return;
            }

            if(deckLeader.factionType != leader.factionType)
            {
                GlobalActions.OnDisplayFeedbackInUI?.Invoke(GlobalConstantValues.MESSAGE_INVALIDLEADERSWITCH);
                return;
            }

            deckLeader = leader;
        }

        public void CloneDeck(Deck deckToClone)
        {
            isCurrentDeck = deckToClone.IsCurrentDeck;

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
                _newCard.cardEffects = _otherCards[i].cardEffects;
                _newCard.cardPower = _otherCards[i].cardPower;
                _newCard.cardType = _otherCards[i].cardType;
                _newCard.maxPerDeck = _otherCards[i].maxPerDeck;
                _newCard.musterTag = _otherCards[i].musterTag;

                cards.Add(_newCard);
            }

            deckLeader = ScriptableObject.CreateInstance<Leader>();
            deckLeader.cardImage = deckToClone.DeckLeader.cardImage;
            deckLeader.factionType = deckToClone.DeckLeader.factionType;
            deckLeader.id = deckToClone.DeckLeader.id;
            deckLeader.abilityDescription = deckToClone.DeckLeader.abilityDescription;
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
                if (card.cardEffects.Contains(EnumCardEffects.Hero))
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