using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_PlayerCardsUIManager : MonoBehaviour
{
    [Header("Prefab Related")]
    public GameObject cardPrefab;
    [SerializeField] private Transform m_cardHolder = null;

    [Header("Play Card Related")]
    [SerializeField] private GameObject m_playCard = null;
    [SerializeField] private AnimationMoveSpotParams m_params = null;

    [Header("Settings Related")]
    [SerializeField] private float availableWidth = 1000f;
    [SerializeField] private float m_cardWidth = 100f;

    [Header("Test Related")]
    public bool m_testEnv = false;
    public int testHandSize = 3;

    private S_GamePlayLogicManager _gameManager = null;
    private UI_GameplayCard m_currentCard = null;
    private List<GameObject> m_cards = new List<GameObject>();
    private List<UI_GameplayCard> m_cardInfo = new List<UI_GameplayCard>();

    private Button m_playBtn;

    private void OnEnable()
    {
        m_playBtn = m_playCard.GetComponentInChildren<Button>();
        var m_hoverBtn = m_playCard.GetComponentInChildren<UI_OnButtonHover>();

        if(m_playCard == null || m_playBtn == null || m_hoverBtn == null || m_params == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "You can't play cards.");
            return;
        }

        m_hoverBtn.InitializeThisUIComp();
        m_playBtn.onClick.AddListener(PlayCardPassToServer);
        GlobalActions.OnClickCard += OnReceiveClickedCard;

        m_playCard.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        m_playBtn.onClick.RemoveListener(PlayCardPassToServer);
        GlobalActions.OnClickCard -= OnReceiveClickedCard;
    }

    private void Start()
    {
        if(m_cardHolder == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Putting card on parent gameObject. This is non-ideal.");
            m_cardHolder = transform;
        }

        if (!m_testEnv) return;

        InitializeHand();
    }

    public void InitializeHand(List<Card> _cardInfo = null, S_GamePlayLogicManager _manager = null)
    {
        int totalCards = testHandSize;

        if(_manager != null)
        {
            _gameManager = _manager;
        }

        if(!m_testEnv && _cardInfo != null)
        {
            totalCards = _cardInfo.Count;
        }

        float _shiftLeft = 1.0f;
        if (totalCards * m_cardWidth > availableWidth) _shiftLeft = (availableWidth / m_cardWidth) / totalCards;

        for (int i = 0; i < totalCards; i++)
        {
            GameObject card = Instantiate(cardPrefab, m_cardHolder);
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

    private void PlayCardPassToServer()
    {
        int _cardSlot = m_currentCard.CardOrder;
        string _cardName = m_currentCard.CardData.id;
        _gameManager.PlayCardDuringTurnServerRpc(_cardName, _cardSlot);
    }

    private void OnReceiveClickedCard(UI_GameplayCard clickedCard, PlayerControls _playerControls)
    {
        if(m_currentCard != null) m_currentCard.Anim.ResetThisObject();

        m_currentCard = clickedCard;
        Card _cardInfo = m_currentCard.CardData;
        m_currentCard.Anim.BeginThisAnimation(m_params);

        if (_cardInfo.unitPlacement == EnumUnitPlacement.Frontline
         || _cardInfo.unitPlacement == EnumUnitPlacement.Ranged
         || _cardInfo.unitPlacement == EnumUnitPlacement.Siege
         || _cardInfo.unitPlacement == EnumUnitPlacement.Global)
        {
            m_playCard.gameObject.SetActive(true);
        }
        else  
        {
            _playerControls.SelectStyle = EnumPlayCardReason.ClickZone;
        }
    }

}
