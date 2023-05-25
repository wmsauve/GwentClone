using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    private UI_GameplayCard m_currentCard;

    private int _cardForward = 1000;
    private void Update()
    {
        ////Used for mousing over cards in the scene. 
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit))
        //{
        //    // Do something with the hit object
        //    //Debug.Log("Hit object: " + hit.transform.name);
        //}

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
        _card.SortOrder = _cardForward;
        m_currentCard = _card;
        return true;
    }
}
