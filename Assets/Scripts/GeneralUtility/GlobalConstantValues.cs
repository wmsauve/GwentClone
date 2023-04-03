using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class GlobalConstantValues : MonoBehaviour
    {
        //Tags
        public const string TAG_MAINCANVAS = "MainUICanvas";

        //Text Labels
        public const string MESSAGE_LOGINMESSAGE = "Enter your Username and Password.";
        public const string MESSAGE_SIGNUPMESSAGE = "Enter a Username and Password to create an account.";
        public const string MESSAGE_SHORTPASSWORD = "Make your password longer than 8 characters.";
        public const string MESSAGE_INPUTFIELDTOOLONG = "Use fewer than 20 characters.";
        public const string MESSAGE_SERVERDOWN = "Server down. Try again later.";

        //Messages
        public const string MESSAGE_DUPLICATEHEROES = "This deck is not allowed copies of this hero card.";
        public const string MESSAGE_NODECKYET = "Create a deck first before trying to add cards.";
        public const string MESSAGE_DUPLICATEREGULAR = "This deck is not allowed more than 2 copies of regular cards.";
        public const string MESSAGE_INVALIDLEADERSWITCH = "The new leader is not part of this deck's faction.";

    }

}
