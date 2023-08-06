using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_GameZone : G_OutlinedGameObject
{

    [Header("Gameplay Related")]
    [SerializeField] private bool m_playerZone;
    [SerializeField] private List<EnumUnitPlacement> m_allowableCards = new List<EnumUnitPlacement>();

    public List<EnumUnitPlacement> AllowableCards { get { return m_allowableCards; } }
    public bool IsPlayerZone { get { return m_playerZone; } }

    private Transform _cardPlace;
    public Transform CardPlace { get { return _cardPlace; } }

    public override void Start()
    {
        base.Start();

        //In the prefab, this appears on the visual aspect of the zone. Cards should be a child of the root.
        _cardPlace = transform.parent;
    }


}
