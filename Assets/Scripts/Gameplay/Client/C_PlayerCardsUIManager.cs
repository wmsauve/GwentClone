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
    [SerializeField] private GameObject m_cancelCard = null;
    [SerializeField] private AnimationMoveSpotParams m_params = null;

    [Header("Settings Related")]
    [SerializeField] private float availableWidth = 1000f;
    [SerializeField] private float m_cardWidth = 100f;

    [Header("Test Related")]
    public bool m_testEnv = false;
    public int testHandSize = 3;

    private S_GamePlayLogicManager _gameManager = null;
    private UI_GameplayCard m_currentCard = null;
    private C_GameZone m_currentZone = null;
    private List<GameObject> m_cards = new List<GameObject>();
    private List<UI_GameplayCard> m_cardInfo = new List<UI_GameplayCard>();

    private Button m_playBtn;
    private Button m_cancelBtn;
    private PlayerControls m_playerControls;
    private EnumUnitPlacement _nonAgilePlacement;
    private bool isAgile = false;

    private void OnEnable()
    {
        if(m_playCard == null || m_cancelCard == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "You didn't place card references for selecting cards.");
            return;
        }

        m_playBtn = m_playCard.GetComponentInChildren<Button>();
        m_cancelBtn = m_cancelCard.GetComponentInChildren<Button>();
        var m_hoverBtn = m_playCard.GetComponentInChildren<UI_OnButtonHover>();
        var m_hoverBtn2 = m_cancelCard.GetComponentInChildren<UI_OnButtonHover>();

        if(m_playBtn == null || m_cancelBtn == null || m_hoverBtn == null || m_hoverBtn2 == null || m_params == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "You can't play cards.");
            return;
        }

        m_playBtn.onClick.AddListener(() => PlayCardPassToServer(isAgile));
        m_cancelBtn.onClick.AddListener(CancelCardSelection);
        GlobalActions.OnClickCard += OnReceiveClickedCard;
        GlobalActions.OnClickZone += OnReceiveClickedZone;

        SetAllButtonsHidden();
    }

    private void OnDisable()
    {
        m_playBtn.onClick.RemoveListener(() => PlayCardPassToServer(isAgile));
        m_cancelBtn.onClick.RemoveListener(CancelCardSelection);
        GlobalActions.OnClickCard -= OnReceiveClickedCard;
        GlobalActions.OnClickZone -= OnReceiveClickedZone;
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
            m_cardInfo[i].SetNewlyAdjustedPositions(i);
        }
    }

    private void PlayCardPassToServer(bool _agileCard = false)
    {
        int _cardSlot = m_currentCard.CardOrder;
        string _cardName = m_currentCard.CardData.id;
        EnumUnitPlacement _cardPlace = _nonAgilePlacement;
        if (_agileCard) _cardPlace = m_currentZone.Zone;
        _gameManager.PlayCardDuringTurnServerRpc(_cardName, _cardSlot, _cardPlace);
    }
    
    public void CancelCardSelection()
    {
        if (m_playerControls == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You should have player controls reference here.");
            return;
        }

        m_playerControls.CancelButtonPressed();
        SetAllButtonsHidden();

        if (m_currentCard != null)
        {
            m_currentCard.Anim.ResetThisObject();
        }

        m_currentCard = null;
        m_currentZone = null;
        isAgile = false;
    }

    public void RemoveCardFromHand(int slot)
    {
        m_cards.RemoveAt(slot);
        m_cardInfo.RemoveAt(slot);
        GameObject playedCard = m_cardHolder.GetChild(slot).gameObject;
        Destroy(playedCard);
        ReadjustCardPositionsInHand();
        SetAllButtonsHidden();
    }

    private void OnReceiveClickedCard(UI_GameplayCard clickedCard, PlayerControls _playerControls)
    {
        if (m_playerControls == null) m_playerControls = _playerControls;
        if (m_currentCard != null) m_currentCard.Anim.ResetThisObject();

        var _myLogic = GeneralPurposeFunctions.GetPlayerLogicReference();
        if (!_myLogic.TurnActive) return;

        m_currentCard = clickedCard;
        Card _cardInfo = m_currentCard.CardData;
        m_currentCard.Anim.BeginThisAnimation(m_params);

        if (_cardInfo.unitPlacement == EnumUnitPlacement.Frontline
         || _cardInfo.unitPlacement == EnumUnitPlacement.Ranged
         || _cardInfo.unitPlacement == EnumUnitPlacement.Siege
         || _cardInfo.unitPlacement == EnumUnitPlacement.Global)
        {
            SetAllButtonsVisible();
            _nonAgilePlacement = _cardInfo.unitPlacement;
        }
        else if(_cardInfo.unitPlacement == EnumUnitPlacement.Any)
        {
            _playerControls.SelectStyle = EnumPlayCardReason.ClickZone;
            m_cancelCard.SetActive(true);
            isAgile = true;
        }
        else if(_cardInfo.unitPlacement == EnumUnitPlacement.SingleTarget)
        {
            _playerControls.SelectStyle = EnumPlayCardReason.SingleTarget;
            m_cancelCard.SetActive(true);
        }
    }

    private void OnReceiveClickedZone(C_GameZone _zone, PlayerControls _playerControls)
    {
        if (m_playerControls == null) m_playerControls = _playerControls;
        m_currentZone = _zone;
        _zone.HideOutline();
        SetAllButtonsVisible();
        _playerControls.SelectStyle = EnumPlayCardReason.ClickCard;
    }

    private void SetAllButtonsVisible()
    {
        m_playCard.SetActive(true);
        m_cancelCard.SetActive(true);
    }
    private void SetAllButtonsHidden()
    {
        m_playCard.SetActive(false);
        m_cancelCard.SetActive(false);
    }

}
