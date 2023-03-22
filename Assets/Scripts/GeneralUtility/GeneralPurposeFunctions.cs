using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class GeneralPurposeFunctions 
    {
        private static string EFFECT_DECOY = "Swap with a card on the battlefield to return it to your hand.";
        private static string EFFECT_HERO = "Not affected by any Special Cards or abilities.";
        private static string EFFECT_MEDIC = "Choose one card from your discard pile and play it instantly (no Heroes or Special Cards).";
        private static string EFFECT_MUSTER = " Find any cards with the same name in your deck and play them instantly.";

        public static Color ReturnColorBasedOnFaction(EnumFactionType faction)
        {
            switch (faction)
            {
                case EnumFactionType.Monsters:
                    return Color.red;
                case EnumFactionType.Neutral:
                    return new Color(0f, 0f, 0f, 0f);
                case EnumFactionType.Nilfgaardian:
                    return Color.black;
                case EnumFactionType.NorthernRealms:
                    return Color.blue;
                case EnumFactionType.Scoiatael:
                    return Color.green;
                case EnumFactionType.Skellige:
                    return Color.magenta;
                default:
                    return new Color(0f, 0f, 0f, 0f);
            }
        }

        public static string ReturnSkillDescription(EnumCardEffects effect)
        {
            switch (effect)
            {
                case EnumCardEffects.Decoy:
                    return EFFECT_DECOY;
                case EnumCardEffects.Hero:
                    return EFFECT_HERO;
                case EnumCardEffects.Medic:
                    return EFFECT_MEDIC;
                case EnumCardEffects.Muster:
                    return EFFECT_MUSTER;
                case EnumCardEffects.None:
                    return "";
                default:
                    return "";
            }
        }
    }

}

