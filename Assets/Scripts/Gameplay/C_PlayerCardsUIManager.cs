using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_PlayerCardsUIManager : MonoBehaviour
{
    [Header("Prefab Related")]
    public GameObject cardPrefab;

    [Header("Settings Related")]
    [SerializeField] private float availableWidth = 1000f;
    [SerializeField] private float m_cardWidth = 100f;

    [Header("Test Related")]
    public bool m_testEnv = false;
    public int testHandSize = 3;

    private List<GameObject> m_cards = new List<GameObject>();
    private List<UI_GameplayCard> m_cardInfo = new List<UI_GameplayCard>();

    private void Start()
    {
        if (!m_testEnv) return;

        InitializeHand();
    }

    public void InitializeHand(List<Card> _cardInfo = null)
    {
        int totalCards = testHandSize;

        if(!m_testEnv && _cardInfo != null)
        {
            totalCards = _cardInfo.Count;
        }

        float _shiftLeft = 1.0f;
        if (totalCards * m_cardWidth > availableWidth) _shiftLeft = (availableWidth / m_cardWidth) / totalCards;

        for (int i = 0; i < totalCards; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform);
            UI_GameplayCard _cardComp = card.GetComponentInChildren<UI_GameplayCard>();
            if (_cardComp == null)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You need a card component on your prefab.");
                break;
            }
            _cardComp.InitializeCardComponent(i, _shiftLeft);
            m_cards.Add(card);
            m_cardInfo.Add(_cardComp);
        }
        ReadjustCardPositionsInHand();

        if (_cardInfo == null) return;
        for(int i = 0; i < totalCards; i++)
        {
            m_cardInfo[i].CardData = _cardInfo[i];
        }
    }

    private float CalculateStartingPosition(int totalCards)
    {
        if(totalCards * m_cardWidth > availableWidth) return -(availableWidth / 2) + (m_cardWidth / 2);
        

        float totalWidth = totalCards * m_cardWidth + (totalCards - 1);
        return -(totalWidth / 2) + (m_cardWidth / 2);
    }

    private void OnTransformChildrenChanged()
    {
        ReadjustCardPositionsInHand();
    }


    private void ReadjustCardPositionsInHand()
    {
        int totalCards = m_cards.Count;
        float startingPosition = CalculateStartingPosition(totalCards);
        float _shiftLeft = 1.0f;

        if (totalCards * m_cardWidth > availableWidth) _shiftLeft = (availableWidth / m_cardWidth) / totalCards;
        
        for (int i = 0; i < totalCards; i++)
        {
            float cardPosition = startingPosition + i * (m_cardWidth * _shiftLeft);
            Vector3 cardLocalPosition = new Vector3(cardPosition, cardPrefab.GetComponent<RectTransform>().rect.height / 2.0f, 0.0f);
            m_cards[i].transform.localPosition = cardLocalPosition;
        }
    }
}
