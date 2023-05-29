using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    private UI_GameplayCard m_currentCard;
    private C_GameZone m_currentZone;
    private int _cardForward = 1000;

    private EnumPlayCardReason _selectStyle = EnumPlayCardReason.ClickZone;
    public EnumPlayCardReason SelectStyle 
    { 
        get { return _selectStyle; } 
        set { _selectStyle = value; } 
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(m_currentCard != null && _selectStyle == EnumPlayCardReason.ClickCard)
            {
                GlobalActions.OnClickCard?.Invoke(m_currentCard, this);
            }

            else if (m_currentZone != null &&_selectStyle == EnumPlayCardReason.ClickZone)
            {
                GlobalActions.OnClickZone?.Invoke(m_currentZone, this);
            }

        }
        


        if(_selectStyle == EnumPlayCardReason.ClickZone)
        {
            //Used for mousing over cards in the scene. 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                C_GameZone _zone = hit.transform.gameObject.GetComponent<C_GameZone>();
                if (_zone == m_currentZone) return;
                if (_zone != null)
                {
                    if (!_zone.PlayerZone) return;

                    if (m_currentZone != null) m_currentZone.HideOutline();

                    m_currentZone = _zone;

                    m_currentZone.ShowOutline();
                }
                else
                {
                    if(m_currentZone != null)
                    {
                        m_currentZone.HideOutline();
                        m_currentZone = null;
                    }
                }
                
                
            }

            return;
        }

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        if(results.Count == 0)
        {
            if(m_currentCard != null)
            {
                m_currentCard.ResetSortOrder();
                m_currentCard = null;
            }
        }
        else DifferentCardHovered(results[0].gameObject);
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
}
