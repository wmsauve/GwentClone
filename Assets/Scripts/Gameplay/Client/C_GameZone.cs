using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_GameZone : MonoBehaviour
{

    [Header("Gameplay Related")]
    [SerializeField] private EnumUnitPlacement m_zone = EnumUnitPlacement.Frontline;
    [SerializeField] private bool m_playerZone = false;

    public bool PlayerZone { get { return m_playerZone; } }
    public EnumUnitPlacement Zone { get { return m_zone; } }

    private Transform _cardPlace;
    public Transform CardPlace { get { return _cardPlace; } }

    private Outline m_myOutline;


    private float _outlineOn = 5f;
    private float _outlineOff = 0f;

    private void Start()
    {
        if(m_zone != EnumUnitPlacement.Frontline && m_zone != EnumUnitPlacement.Ranged && m_zone != EnumUnitPlacement.Siege)
        {
            m_zone = EnumUnitPlacement.Frontline;
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Are you sure you set the zone position correctly?");
        }

        m_myOutline = GetComponent<Outline>();
        if(m_myOutline == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Your Zone doesnt have an Outliner.");
            return;
        }
        m_myOutline.OutlineWidth = _outlineOff;

        //In the prefab, this appears on the visual aspect of the zone. Cards should be a child of the root.
        _cardPlace = transform.parent;
    }

    public void ShowOutline()
    {
        m_myOutline.OutlineWidth = _outlineOn;
    }
    public void HideOutline()
    {
        m_myOutline.OutlineWidth = _outlineOff;
    }


    public void AddCardToZone()
    {

    }

    public void RemoveCardFromZone()
    {

    }

    private void RePositionCardsInZone()
    {

    }

}
