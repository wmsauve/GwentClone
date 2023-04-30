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
        [Header("Identifier Related")]
        [Tooltip("Name of the card, but also used as identifier in code.")]
        public string id;
        [Tooltip("Sprite of card.")]
        public Sprite cardImage;

        [Header("Card Information Related")]
        public EnumUnitType unitType;
        public EnumCardType cardType;
        public EnumUnitPlacement unitPlacement;
        public EnumFactionType factionType;
        public List<EnumCardEffects> cardEffects;

        [Header("Interact With Mechanics Related")]
        public int cardPower;
        public int maxPerDeck;
        [Tooltip("Used for mustering. Usually the same as ID. Can be different if a specific muster mechanic is utilized.")]
        public string musterTag;
        [Tooltip("Specify scorch target since cards can either target their placement or any placement for scorching.")]
        public EnumUnitPlacement scorchTarget;
        
    }

}

