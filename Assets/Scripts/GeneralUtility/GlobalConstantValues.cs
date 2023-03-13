using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class GlobalConstantValues : MonoBehaviour
    {
        //Tags
        public const string TAG_MAINCANVAS = "MainUICanvas";

        //Messages
        public const string MESSAGE_DUPLICATEHEROES = "This deck is not allowed copies of this hero card.";
        public const string MESSAGE_NODECKYET = "Create a deck first before trying to add cards.";
    }

}
