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
        public static Deck CurrentDeck { get { return currentDeck; } }
        private static Deck cloneDeck;
        public static Deck GetCloneDeck { get { return cloneDeck; } }

        public static Deck AddDeck(Leader leader)
        {
            var _newDeck = new Deck();
            var _newName = "NEW DECK";
            _newDeck.SetDeckLeader(leader);
            _newDeck.SetDeckName(_newName.ToUpper().Trim());
            myDecks.Add(_newDeck);
            if (currentDeck == null) SwitchFocusedDeck(_newDeck);
            return _newDeck;
        }

        public static bool AddCardToCurrentDeck(Card newCard)
        {
            if(currentDeck == null)
            {
                GlobalActions.OnDisplayFeedbackInUI?.Invoke(GlobalConstantValues.MESSAGE_NODECKYET);
                return false;
            }

            var _validCard = RunCheckForValidCardAdd(newCard);
            if (!_validCard) return false;

            currentDeck.AddCard(newCard);
            var _status = RunCheckForDeckChange();
            MainMenu_DeckSaved.DeckChangedStatus = _status;
            return true;
        }

        public static bool RemoveCardFromCurrentDeck(Card newCard)
        {
            if (currentDeck == null)
            {
                GlobalActions.OnDisplayFeedbackInUI?.Invoke(GlobalConstantValues.MESSAGE_NODECKYET);
                return false;
            }

            currentDeck.RemoveCard(newCard);
            var _status = RunCheckForDeckChange();
            MainMenu_DeckSaved.DeckChangedStatus = _status;
            return true;
        }

        public static void SwitchFocusedDeck(Deck newFocused)
        {
            if (currentDeck != null)
            {
                if (newFocused.DeckUID == currentDeck.DeckUID) return;
            }

            currentDeck = newFocused;
            cloneDeck = new Deck();
            cloneDeck.CloneDeck(newFocused);

#if UNITY_EDITOR
            Debug.LogWarning("=======================");
            foreach(Card card in newFocused.Cards)
            {
                Debug.LogWarning(card.id + " card in this deck.");
            }
            Debug.LogWarning("=======================");
#endif

        }

        public static EnumDeckStatus RunCheckForDeckChange()
        {
            var newDeckName = currentDeck.DeckName;
            var cloneDeckName = cloneDeck.DeckName;

            if (!newDeckName.Equals(cloneDeckName)) return EnumDeckStatus.Changed;
            var newCards = currentDeck.Cards;
            var oldCards = cloneDeck.Cards;

            if (newCards == null) return EnumDeckStatus.NotChanged;

            if (oldCards.Count != newCards.Count) return EnumDeckStatus.Changed;

            for (int i = 0; i < newCards.Count; i++)
            {
                if (oldCards == null) return EnumDeckStatus.Changed;
                if (oldCards.Count == 0) return EnumDeckStatus.Changed;
                if (newCards[i].cardImage != oldCards[i].cardImage) return EnumDeckStatus.Changed;
                if (newCards[i].id != oldCards[i].id) return EnumDeckStatus.Changed;
                if (newCards[i].unitType != oldCards[i].unitType) return EnumDeckStatus.Changed;
                if (newCards[i].cardType != oldCards[i].cardType) return EnumDeckStatus.Changed;
                if (newCards[i].unitPlacement != oldCards[i].unitPlacement) return EnumDeckStatus.Changed;
                if (newCards[i].factionType != oldCards[i].factionType) return EnumDeckStatus.Changed;
                if (newCards[i].cardEffects != oldCards[i].cardEffects) return EnumDeckStatus.Changed;
                if (newCards[i].isHero != oldCards[i].isHero) return EnumDeckStatus.Changed;
                if (newCards[i].cardPower != oldCards[i].cardPower) return EnumDeckStatus.Changed;
                if (newCards[i].maxPerDeck != oldCards[i].maxPerDeck) return EnumDeckStatus.Changed;
                if (newCards[i].musterTag != oldCards[i].musterTag) return EnumDeckStatus.Changed;
            }

            return EnumDeckStatus.NotChanged;
        }

        public static void DeckSaved()
        {
            cloneDeck = new Deck();
            cloneDeck.CloneDeck(currentDeck);
        }

        public static Deck RevertCurrentDeckToClone()
        {
            currentDeck = new Deck();
            currentDeck.CloneDeck(cloneDeck);

            return currentDeck;
        }

        private static bool RunCheckForValidCardAdd(Card newCard)
        {
            int bronzeCounter = 0;

            if (currentDeck.Cards.Count > 0)
            {
                foreach (Card card in currentDeck.Cards)
                {

                    if (newCard.isHero && newCard.id == card.id)
                    {
                        GlobalActions.OnDisplayFeedbackInUI?.Invoke(GlobalConstantValues.MESSAGE_DUPLICATEHEROES);
                        return false;
                    }

                    if (!newCard.isHero && newCard.id == card.id)
                    {
                        bronzeCounter++;
                        continue;
                    }
                }
            }

            if (bronzeCounter == 2)
            {
                GlobalActions.OnDisplayFeedbackInUI?.Invoke(GlobalConstantValues.MESSAGE_DUPLICATEREGULAR);
                return false;
            }


            return true;
        }

        public static string GetCurrentDeckGUID()
        {
            return currentDeck.DeckUID;
        }
        public static string GetUnsavedDeckName()
        {
            return cloneDeck.DeckName;
        }
    }

}

