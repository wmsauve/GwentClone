using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public static class MainMenu_DeckManager
    {

        private static List<Deck> myDecks = new List<Deck>();

        public static List<Deck> MyDecks { get { return myDecks; } }

        private static Deck currentDeck;
        private static Deck cloneDeck;

        public static Deck AddDeck()
        {
            var _newDeck = new Deck();
            var _newName = "New Deck";
            _newDeck.SetDeckName(_newName.ToUpper().Trim());
            myDecks.Add(_newDeck);
            if (currentDeck == null) SwitchFocusedDeck(_newDeck);
            return _newDeck;
        }

        public static void SwitchFocusedDeck(Deck newFocused)
        {
            currentDeck = newFocused;
            cloneDeck = new Deck();
            cloneDeck.CloneDeck(newFocused);
        }

        public static EnumDeckStatus RunCheckForDeckChange()
        {
            var newDeckName = currentDeck.DeckName;
            var cloneDeckName = cloneDeck.DeckName;

            Debug.LogWarning(newDeckName + " new deck");
            Debug.LogWarning(newDeckName.Equals("NEW DECK") + " new deck vs string");

            Debug.LogWarning(cloneDeckName.Equals("NEW DECK") + " clone deck vs string");

            if (!newDeckName.Equals(cloneDeckName)) return EnumDeckStatus.Changed;
            var newCards = currentDeck.Cards;
            var oldCards = cloneDeck.Cards;

            if (newCards == null || newCards.Count == 0) return EnumDeckStatus.NotChanged;

            for (int i = 0; i < newCards.Count; i++)
            {
                if (oldCards[i] == null) return EnumDeckStatus.Changed;
                if (newCards[i].cardImage != oldCards[i].cardImage) return EnumDeckStatus.Changed;
                if (newCards[i].id != oldCards[i].id) return EnumDeckStatus.Changed;
                if (newCards[i].unitType != oldCards[i].unitType) return EnumDeckStatus.Changed;
                if (newCards[i].cardType != oldCards[i].cardType) return EnumDeckStatus.Changed;
                if (newCards[i].unitPlacement != oldCards[i].unitPlacement) return EnumDeckStatus.Changed;
                if (newCards[i].factionType != oldCards[i].factionType) return EnumDeckStatus.Changed;
                if (newCards[i].specialEffect != oldCards[i].specialEffect) return EnumDeckStatus.Changed;
                if (newCards[i].isHero != oldCards[i].isHero) return EnumDeckStatus.Changed;
                if (newCards[i].cardPower != oldCards[i].cardPower) return EnumDeckStatus.Changed;
                if (newCards[i].maxPerDeck != oldCards[i].maxPerDeck) return EnumDeckStatus.Changed;
            }

            return EnumDeckStatus.NotChanged;
        }

        public static void DeckSaved()
        {
            cloneDeck = new Deck();
            cloneDeck.CloneDeck(currentDeck);
        }
    }

}

