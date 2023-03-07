using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class Deck
    {
        private List<Card> cards;
        public List<Card> Cards { get { return cards; } }


        public Deck() { }

        public void AddCard(Card _card)
        {
            cards.Add(_card);
        }
    }

}

