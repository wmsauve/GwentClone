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
        private static string EFFECT_TIGHTBOND = "When placed next to a card with the same name, doubles the strength of both (or more) cards.";
        private static string EFFECT_SPY = "Can be placed on your opponent's battlefield (and count towards your opponent's total) but allow you to draw 2 extra cards from your deck.";
        private static string EFFECT_MORALEBOOST = "Add +1 strength to all units in the row in which they are played, excluding themselves.";
        private static string EFFECT_AGILE = "Can be placed in either the Close Combat or the Ranged Combat row. Cannot be moved once placed.";
        private static string EFFECT_SCORCHRANGED = "Destroy your enemy's strongest Ranged Combat unit(s) if the combined strength of all his or her Ranged Combat units is 10 or more.";
        private static string EFFECT_CLOSECOMBATWEATHER = "Sets the strength of all Close Combat cards to 1 for both players.";
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

        public static string ReturnSkillDescription(EnumCardEffects effect, EnumUnitPlacement placement = EnumUnitPlacement.Any)
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
                case EnumCardEffects.TightBond:
                    return EFFECT_TIGHTBOND;
                case EnumCardEffects.Spy:
                    return EFFECT_SPY;
                case EnumCardEffects.MoraleBoost:
                    return EFFECT_MORALEBOOST;
                case EnumCardEffects.Agile:
                    return EFFECT_AGILE;
                case EnumCardEffects.Scorch:
                    if (placement == EnumUnitPlacement.Ranged) return EFFECT_SCORCHRANGED;
                    return "";
                case EnumCardEffects.Weather:
                    if (placement == EnumUnitPlacement.Frontline) return EFFECT_CLOSECOMBATWEATHER;
                    return "";
                case EnumCardEffects.None:
                    return "";
                default:
                    return "";
            }
        }
    }

}

