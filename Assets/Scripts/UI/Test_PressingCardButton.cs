using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GwentClone;
using TMPro;

public class Test_PressingCardButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_cardName; 

    private void OnEnable()
    {
        GlobalActions.OnPressCardButton += OnCardPressed;
    }

    private void OnDisable()
    {
        GlobalActions.OnPressCardButton -= OnCardPressed;
    }

    private void OnCardPressed(Card _cardData)
    {
        if (m_cardName == null) return;

        m_cardName.text = _cardData.id;
    }

}
