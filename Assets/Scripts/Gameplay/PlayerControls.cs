using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    private UI_GameplayCard m_currentCard;
    private C_GameZone m_currentZone;
    private C_PlayedCard m_currentTarget;
    private int _cardForward = 1000;
    private C_PlayerGamePlayLogic _myLogic;

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

    private void Start()
    {
        _myLogic = GeneralPurposeFunctions.GetPlayerLogicReference();
        if (_myLogic == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Your controller should have reference to player logic.");
            return;
        }
    }

    private void Update()
    {
        if (!_myLogic.TurnActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            if(m_currentCard != null && SelectStyle == EnumPlayerControlsStatus.ClickCard)
            {
                GlobalActions.OnClickCard?.Invoke(m_currentCard, this);
                SelectStyle = EnumPlayerControlsStatus.CarryingCard;
            }

            //else if (m_currentZone != null &&_selectStyle == EnumPlayCardReason.ClickZone)
            //{
            //    GlobalActions.OnClickZone?.Invoke(m_currentZone, this);
            //}
        }

        if (Input.GetMouseButtonUp(0))
        {
            GlobalActions.OnCardInteractionInGame?.Invoke(_currentDropCardStatus);
            SelectStyle = EnumPlayerControlsStatus.ClickCard;
        }

        //if(SelectStyle == EnumPlayCardReason.SingleTarget)
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        C_PlayedCard _card = hit.transform.gameObject.GetComponent<C_PlayedCard>();
        //        if (_card == m_currentTarget) return;
        //        if (_card != null)
        //        {
        //            if (m_currentTarget != null) m_currentZone.HideOutline();
        //            m_currentTarget = _card;
        //            m_currentTarget.ShowOutline();
        //        }
        //        else
        //        {
        //            if (m_currentTarget != null)
        //            {
        //                m_currentTarget.HideOutline();
        //                m_currentTarget = null;
        //            }
        //        }
        //    }
        //}

        switch (SelectStyle)
        {
            case EnumPlayerControlsStatus.ClickCard:
                HoverCardsInHand();
                break;
            case EnumPlayerControlsStatus.CarryingCard:

                if (m_currentCard == null) break;

                var _data = m_currentCard.CardData;

                if (GeneralPurposeFunctions.PlayCardOnDrop(EnumPlayCardStatus.PlayToZone, _data.unitPlacement))
                {
                    var success = FindCardZone();
                    if (success) break;
                }


                _currentDropCardStatus = EnumDropCardReason.Nothing;
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

    public void CancelButtonPressed()
    {
        SelectStyle = EnumPlayerControlsStatus.ClickCard;

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
                //Successfully found our play area.
                if (_zone.AllowableCards.Contains(m_currentCard.CardData.unitPlacement))
                {
                    if (m_currentZone != null) m_currentZone.HideOutline();
                    m_currentZone = _zone;
                    m_currentZone.ShowOutline();
                    _currentDropCardStatus = EnumDropCardReason.PlayMinion;
                    return true;
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
                return false;
            }
        }
        return false;
    }
}
