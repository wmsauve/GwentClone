using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GwentClone;
using TMPro;

public class UI_DeckInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_totalCards;
    [SerializeField] private TextMeshProUGUI m_numOfUnits;
    [SerializeField] private TextMeshProUGUI m_specialCards;
    [SerializeField] private TextMeshProUGUI m_totalStrength;
    [SerializeField] private TextMeshProUGUI m_heroCards;

    private int cacheCardAmount = 0;

    private void Start()
    {
        if(m_totalCards == null || m_numOfUnits == null || m_specialCards == null || m_totalStrength == null || m_heroCards == null)
        {
            Debug.LogWarning("You didn't put references for the deck info display.");
        }
    }

    private void Update()
    {
        if (m_totalCards == null || m_numOfUnits == null || m_specialCards == null || m_totalStrength == null || m_heroCards == null)
        {
            return;
        }

        if (MainMenu_DeckManager.CurrentDeck == null) return;

        if (MainMenu_DeckManager.CurrentDeck.TotalCards != cacheCardAmount)
        {
            OnUpdateUIInfo();
        }
    }

    private void OnUpdateUIInfo()
    {
        var currentInfo = MainMenu_DeckManager.CurrentDeck;
        m_totalCards.text = currentInfo.TotalCards.ToString();
        cacheCardAmount = currentInfo.TotalCards;

        m_numOfUnits.text = currentInfo.NumberOfUnits.ToString();
        m_specialCards.text = currentInfo.SpecialCards.ToString() + "/10";
        m_totalStrength.text = currentInfo.TotalStrength.ToString();
        m_heroCards.text = currentInfo.HeroCards.ToString();
    }

}
