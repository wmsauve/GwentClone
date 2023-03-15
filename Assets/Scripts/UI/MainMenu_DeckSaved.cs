using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public static class MainMenu_DeckSaved 
    {
        private static EnumDeckStatus deckChangedStatus = EnumDeckStatus.NotChanged;
        public static EnumDeckStatus DeckChangedStatus { 
            get { return deckChangedStatus; } 
            set {
                deckChangedStatus = value;
                GlobalActions.OnDeckChanged?.Invoke(deckChangedStatus);
            } 
        } 


    }

}
