using UnityEngine;
using System;


[Serializable]
[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/GwentLeader")]
public class Leader : ScriptableObject
{
    public string id;
    public EnumFactionType factionType;
    public Sprite cardImage;
    [TextArea(3, 10)]
    public string abilityDescription;
}

