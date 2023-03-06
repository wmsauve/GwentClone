using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GwentClone
{
    [Serializable]
    [CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/GwentCard")]
    public class Card : ScriptableObject
    {
        public string id;

        public EnumUnitType unitType;
        public EnumCardType cardType;
        public bool specialEffect;
        public int cardPower;

        public Sprite cardImage;
    }

}

