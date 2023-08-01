using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_ZonesManager : MonoBehaviour
{

    [SerializeField] private List<C_GameZone> m_playerZones = new List<C_GameZone>();
    [SerializeField] private List<C_GameZone> m_opponentZones = new List<C_GameZone>();
    [SerializeField] private int m_playerZone = 3;
    [SerializeField] private int m_opponentZone = 3;

    [SerializeField] private GameObject _placedCardPrefab;

    private C_PlayerGamePlayLogic _myLogic;

    private void Start()
    {

        if(_placedCardPrefab == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You are missing a component for placing cards in zone.");
            return;
        }

        if(m_playerZones.Count != m_playerZone || m_opponentZones.Count != m_opponentZone)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Did you add the correct number of zones?");
            return;
        }
    }

    public void AddCardToZone(Card _cardData, EnumUnitPlacement _cardPlacement)
    {
        if(_myLogic == null) _myLogic = GeneralPurposeFunctions.GetPlayerLogicReference();

        var _whichZones = _myLogic.TurnActive ? m_playerZones : m_opponentZones;
        //EnumPlayCardReason _outlineCondition = _myLogic.TurnActive ? EnumPlayCardReason.SingleTarget : EnumPlayCardReason.SingleTargetEnemy;
        var _zone = _whichZones.Find((x) => _cardPlacement == x.Zone);
        if (_zone != null)
        {
            var newCard = Instantiate(_placedCardPrefab, _zone.CardPlace);
            var _cardComp = newCard.GetComponent<C_PlayedCard>();
            if (_cardComp != null) _cardComp.InitializePlayedCard(_cardData);
        }
    }

    public void CleanZones()
    {
        //GetChild(1) - Outline is GetChild(0)

        for(int i = 0; i < m_playerZones.Count; i++)
        {
            var _cardPlace = m_playerZones[i].CardPlace;
            for(int j = 1; j < _cardPlace.childCount; j++)
            {
                Destroy(_cardPlace.GetChild(j).gameObject); 
            }
        }

        for (int i = 0; i < m_opponentZones.Count; i++)
        {
            var _cardPlace = m_opponentZones[i].CardPlace;
            for (int j = 1; j < _cardPlace.childCount; j++)
            {
                Destroy(_cardPlace.GetChild(j).gameObject);
            }
        }
    }

    public void DestroyCardsOfPowerDueToScorch(int _powerToDestroy)
    {
        //For now just check global. For scorch effects on cards, need to know opponent or enemy.

        for (int i = 0; i < m_playerZones.Count; i++)
        {
            var _cardPlace = m_playerZones[i].CardPlace;
            for (int j = _cardPlace.childCount - 1; j > 0; j--)
            {
                var _cardComp = _cardPlace.GetChild(j).gameObject.GetComponent<C_PlayedCard>();
                if(_cardComp == null)
                {
                    GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "How does this card not have a comp?");
                    return;
                }
                if (_cardComp.MyCard.cardPower == _powerToDestroy) Destroy(_cardPlace.GetChild(j).gameObject);
            }
        }

        for (int i = 0; i < m_opponentZones.Count; i++)
        {
            var _cardPlace = m_opponentZones[i].CardPlace;
            for (int j = _cardPlace.childCount - 1; j > 0; j--)
            {
                var _cardComp = _cardPlace.GetChild(j).gameObject.GetComponent<C_PlayedCard>();
                if (_cardComp == null)
                {
                    GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "How does this card not have a comp?");
                    return;
                }
                if (_cardComp.MyCard.cardPower == _powerToDestroy) Destroy(_cardPlace.GetChild(j).gameObject);
            }
        }
    }
}
