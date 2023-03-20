
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

        /// <summary>
        /// When pressing the top deck buttons.
        /// </summary>
        public static Action<Deck> OnPressDeckChangeButton;

        /// <summary>
        /// Used to pass either card or leader data for user's tooltip needs.
        /// </summary>
        public static Action<Card, Leader> OnHoveredUIButton;
        public static Action OnStopHoveredUIButton;
    }
    
}

