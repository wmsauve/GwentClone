using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_GameZone : G_OutlinedGameObject
{

    [Header("Gameplay Related")]
    [SerializeField] private EnumUnitPlacement m_zone = EnumUnitPlacement.Frontline;
    [SerializeField] private bool m_playerZone = false;

    public bool PlayerZone { get { return m_playerZone; } }
    public EnumUnitPlacement Zone { get { return m_zone; } }

    private Transform _cardPlace;
    public Transform CardPlace { get { return _cardPlace; } }

    public override void Start()
    {
        base.Start();

        if(m_zone != EnumUnitPlacement.Frontline && m_zone != EnumUnitPlacement.Ranged && m_zone != EnumUnitPlacement.Siege)
        {
            m_zone = EnumUnitPlacement.Frontline;
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Are you sure you set the zone position correctly?");
        }

        //In the prefab, this appears on the visual aspect of the zone. Cards should be a child of the root.
        _cardPlace = transform.parent;
    }


}
