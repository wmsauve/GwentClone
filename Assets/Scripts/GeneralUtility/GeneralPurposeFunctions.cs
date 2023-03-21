using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class GeneralPurposeFunctions 
    {
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
    }

}

