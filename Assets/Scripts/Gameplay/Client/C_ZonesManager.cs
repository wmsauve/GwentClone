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
        if(_myLogic == null)
        {
            var logics = FindObjectsOfType<C_PlayerGamePlayLogic>();
            if (logics.Length == 0) GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Should have player logic");
            for(int i = 0; i < logics.Length; i++)
            {
                if (logics[i].ReturnOwnerStatus())
                {
                    _myLogic = logics[i];
                    break;
                }
            }
        }

        

        var _whichZones = _myLogic.TurnActive ? m_playerZones : m_opponentZones;

        var _zone = _whichZones.Find((x) => _cardPlacement == x.Zone);
        if (_zone != null)
        {
            var newCard = Instantiate(_placedCardPrefab, _zone.CardPlace);
            var _cardComp = newCard.GetComponent<C_PlayedCard>();
            if (_cardComp != null)
            {
                _cardComp.InitializePlayedCard(_cardData);
            }
        }

        Debug.LogWarning("Are we getting here?");
    }
}
