using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_GameZone : G_OutlinedGameObject
{

    [Header("Gameplay Related")]
    [SerializeField] private bool m_playerZone;
    [SerializeField] private List<EnumUnitPlacement> m_allowableCards = new List<EnumUnitPlacement>();

    [Header("Settings Related")]
    [SerializeField] private float availableWidth = 10f;
    [SerializeField] private float m_cardWidth = 1;
    [SerializeField] private float m_cardYPos = 0.2f;
    [SerializeField] private float m_padding = 0.1f;

    public List<EnumUnitPlacement> AllowableCards { get { return m_allowableCards; } }
    public bool IsPlayerZone { get { return m_playerZone; } }

    private Transform _cardPlace;
    public Transform CardPlace { get { return _cardPlace; } }

    public override void Start()
    {
        base.Start();

        //In the prefab, this appears on the visual aspect of the zone. Cards should be a child of the root.
        _cardPlace = transform.parent;

        m_cardWidth += (m_padding * 2);
    }


    private float CalculateStartingPosition(int totalCards)
    {
        if (totalCards * m_cardWidth > availableWidth) return -(availableWidth / 2) + (m_cardWidth / 2);


        float totalWidth = totalCards * m_cardWidth;
        return -(totalWidth / 2) + (m_cardWidth / 2);
    }

    public void ReadjustCardPositionsInZone()
    {
        int totalCards = _cardPlace.childCount - 1;
        float startingPosition = CalculateStartingPosition(totalCards);
        float _shiftLeft = 1.0f;

        if (totalCards * m_cardWidth > availableWidth) _shiftLeft = (availableWidth / m_cardWidth) / totalCards;

        for (int i = 0; i < totalCards; i++)
        {
            float cardPosition = startingPosition + i * (m_cardWidth * _shiftLeft);
            Vector3 cardLocalPosition = new Vector3(0f, m_cardYPos, cardPosition); //Because I'm fucking dumb, Red axis is forward facing.
            _cardPlace.GetChild(i + 1).transform.localPosition = cardLocalPosition;
        }
    }
}
