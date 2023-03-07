using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public static class MainMenu_DeckManager
    {

        private static List<Deck> myDecks = new List<Deck>();

        public static List<Deck> MyDecks { get { return myDecks; } }

        public static void AddDeck()
        {
            myDecks.Add(new Deck());
        }
    }

}

