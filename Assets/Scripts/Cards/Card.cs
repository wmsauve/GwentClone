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
        public EnumUnitPlacement unitPlacement;
        public EnumFactionType factionType;
        public bool specialEffect;
        public bool isHero;
        public int cardPower;
        public int maxPerDeck;


        public Sprite cardImage;
    }

}
