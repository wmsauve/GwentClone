using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    private UI_GameplayCard m_currentCard;
    private C_GameZone m_currentZone;
    private C_GameZone[] m_allZones;
    private C_PlayedCard m_currentTarget;
    private int _cardForward = 1000;
    private C_PlayerGamePlayLogic _myLogic;

    private bool _playerControls = true;
    private bool _globalHover = false;

    private EnumPlayerControlsStatus _selectStyle = EnumPlayerControlsStatus.ClickCard;
    public EnumPlayerControlsStatus SelectStyle 
    { 
        get { return _selectStyle; } 
        set 
        { 
            _selectStyle = value;
            GlobalActions.OnClickModeChange?.Invoke(_selectStyle);
        } 
    }

    private EnumDropCardReason _currentDropCardStatus;

    private EnumPlayCardStatus _currentPlayLocation;

    private void OnEnable()
    {
        GlobalActions.OnCardInteractionInGame += CardNotHeldAnymore;
    }

    private void OnDisable()
    {
        GlobalActions.OnCardInteractionInGame -= CardNotHeldAnymore;
    }

    private void Start()
    {
        _myLogic = GeneralPurposeFunctions.GetPlayerLogicReference();
        if (_myLogic == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Your controller should have reference to player logic.");
            return;
        }

        //Stop player from controlling the player controls on all players - consider removing this component.
        var sharedLogic = GetComponent<C_PlayerGamePlayLogic>();
        if (_myLogic != sharedLogic) _playerControls = false;

        m_allZones = GeneralPurposeFunctions.GetComponentsFromScene<C_GameZone>();
    }

    private void Update()
    {
        if (!_playerControls) return;

        if (Input.GetMouseButtonDown(0))
        {
            if(m_currentCard != null && SelectStyle == EnumPlayerControlsStatus.ClickCard)
            {
                m_currentCard.ResetSortOrder();
                GlobalActions.OnClickCard?.Invoke(m_currentCard, this);
                SelectStyle = EnumPlayerControlsStatus.CarryingCard;
                _currentPlayLocation = GeneralPurposeFunctions.GetIntendedPlayLocation(m_currentCard.CardData);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            SelectStyle = EnumPlayerControlsStatus.ClickCard;

            if (_currentDropCardStatus == EnumDropCardReason.Nothing)
            {
                GlobalActions.OnNotPlayingHeldCard?.Invoke();
                return;
            }

            GlobalActions.OnCardInteractionInGame?.Invoke(m_currentZone, _currentDropCardStatus);
            _currentDropCardStatus = EnumDropCardReason.Nothing;
            _globalHover = false;
        }

        switch (SelectStyle)
        {
            case EnumPlayerControlsStatus.ClickCard:
                HoverCardsInHand();
                break;
            case EnumPlayerControlsStatus.CarryingCard:

                if (m_currentCard == null) break;

                var _data = m_currentCard.CardData;

                if (GeneralPurposeFunctions.PlayCardOnDrop(_currentPlayLocation, _data.unitPlacement))
                {
                    var success = FindCardZone();
                    if (success) break;
                }

                break;
        }
    }

    private void DifferentCardHovered(GameObject _newCard)
    {
        if(m_currentCard == null)
        {
            GetCardComponentFromObject(_newCard);
            return;
        }

        m_currentCard.ResetSortOrder();
        GetCardComponentFromObject(_newCard);
    }

    private bool GetCardComponentFromObject(GameObject _obj)
    {
        var _card = _obj.GetComponent<UI_GameplayCard>();
        if (_card == null) return false;
        _card.CardOrder = _cardForward;
        m_currentCard = _card;
        return true;
    }

    private void HoverCardsInHand()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count == 0)
        {
            if (m_currentCard != null)
            {
                m_currentCard.ResetSortOrder();
                m_currentCard = null;
            }
        }
        else DifferentCardHovered(results[0].gameObject);
    }

    private bool FindCardZone()
    {
        //Used for mousing over cards in the scene. 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            C_GameZone _zone = hit.transform.gameObject.GetComponent<C_GameZone>();
            if (_zone == m_currentZone) return false;
            if (_zone != null)
            {
                EnumUnitPlacement _placement = m_currentCard.CardData.unitPlacement;

                //Successfully found our play area.
                if (_zone.AllowableCards.Contains(_placement) && _placement != EnumUnitPlacement.Global)
                {
                    if (m_currentZone != null) m_currentZone.HideOutline();
                    m_currentZone = _zone;
                    m_currentZone.ShowOutline();
                    _currentDropCardStatus = EnumDropCardReason.PlayMinion;
                    return true;
                }

                if(_placement == EnumUnitPlacement.Global)
                {
                    if(!_globalHover)
                    {
                        m_currentZone = _zone;
                        for(int i = 0; i < m_allZones.Length; i++)
                        {
                            m_allZones[i].ShowOutline();
                        }
                        _currentDropCardStatus = EnumDropCardReason.PlayGlobal;
                        _globalHover = true;
                        return true;
                    }
                }

                return false;
            }
            else
            {
                if (m_currentZone != null)
                {
                    m_currentZone.HideOutline();
                    m_currentZone = null;
                }

                if (_globalHover)
                {
                    m_currentZone = null;
                    for (int i = 0; i < m_allZones.Length; i++)
                    {
                        m_allZones[i].HideOutline();
                    }
                    _globalHover = false;
                }

                _currentDropCardStatus = EnumDropCardReason.Nothing;
                return false;
            }
        }
        return false;
    }

    private void CardNotHeldAnymore(C_GameZone _zone, EnumDropCardReason _reason)
    {
        for (int i = 0; i < m_allZones.Length; i++)
        {
            m_allZones[i].HideOutline();
        }
    }
}
