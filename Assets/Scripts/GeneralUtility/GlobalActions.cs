
using System;

namespace GwentClone
{
    public static class GlobalActions
    {
        /// <summary>
        /// Invoked to initialize UI objects.
        /// </summary>
        public static Action OnInitializeAllUI;

        /// <summary>
        /// Use this for pressing card buttons.
        /// </summary>
        public static Action<Card> OnPressCardButton;

        /// <summary>
        /// Invoke in MainMenu_DeckSaved.
        /// </summary>
        public static Action<EnumDeckStatus> OnDeckChanged;

        /// <summary>
        /// Provide feedback when a user input is invalid for some reason.
        /// </summary>
        public static Action<string> OnDisplayFeedbackInUI;
    }
    
}

