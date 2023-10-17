using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "GwentDeck", menuName = "ScriptableObjects/GwentDeck")]
public class GwentDeck : ScriptableObject
{
    public Leader m_deckLeader;

    public List<Card> m_card;
}
