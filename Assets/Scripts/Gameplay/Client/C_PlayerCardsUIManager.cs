using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_PlayerCardsUIManager : MonoBehaviour
{
    [Header("Prefab Related")]
    public GameObject cardPrefab;
    [SerializeField] private Transform m_cardHolder = null;

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

    private PlayerControls m_playerControls;
    private EnumUnitPlacement _nonAgilePlacement;
    private bool isAgile = false;

    private RectTransform canvasRect = null;
    private Transform parentToMainRect = null;
    private RectTransform mainRect = null;

    private void OnEnable()
    {
        GlobalActions.OnClickCard += OnReceiveClickedCard;
        GlobalActions.OnCardInteractionInGame += OnStopClickingCard;
        GlobalActions.OnNotPlayingHeldCard += OnNotPlayingHeldCard;
    }

    private void OnDisable()
    {
        GlobalActions.OnClickCard -= OnReceiveClickedCard;
        GlobalActions.OnCardInteractionInGame -= OnStopClickingCard;
        GlobalActions.OnNotPlayingHeldCard -= OnNotPlayingHeldCard;
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

    private void Update()
    {
        if (m_currentCard == null) return;

        if (canvasRect == null || mainRect == null) return;

        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out anchoredPos);

        // Adjust for the pivot point of the UI element
        anchoredPos.x += mainRect.rect.width * mainRect.pivot.x;
        anchoredPos.y += mainRect.rect.height * mainRect.pivot.y;

        mainRect.anchoredPosition = anchoredPos;
    }

    public void InitializeHand(List<Card> _cardInfo = null, S_GamePlayLogicManager _manager = null)
    {
        int totalCards = testHandSize;

        if(_manager != null) _gameManager = _manager;
        if(!m_testEnv && _cardInfo != null) totalCards = _cardInfo.Count;
        
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

    private void PlayCardPassToServer(EnumUnitPlacement _placement, S_GamePlayLogicManager.InteractTarget[] _interactedCards = null)
    {
        int _cardSlot = m_currentCard.CardOrder;
        string _cardName = m_currentCard.CardData.id;
        if (_interactedCards == null || _interactedCards.Length == 0) _gameManager.PlayCardDuringTurnServerRpc(_cardName, _cardSlot, _placement);
        else
        {
            var _json = GeneralPurposeFunctions.ConvertArrayToJson(_interactedCards);
            Debug.LogWarning(_json + " json on client.");
            _gameManager.PlayCardDuringTurnServerRpc(_cardName, _cardSlot, _placement, _json);
        }
    }

    public void RemoveCardFromHand(int slot)
    {
        m_cards.RemoveAt(slot);
        m_cardInfo.RemoveAt(slot);
        GameObject playedCard = m_cardHolder.GetChild(slot).gameObject;
        Destroy(playedCard);
        ReadjustCardPositionsInHand();
    }

    public void SwapCardInHand(int slot, Card card)
    {
        UI_GameplayCard _cardInHand = m_cardInfo[slot];
        _cardInHand.CardData = card;
    }

    private void OnReceiveClickedCard(UI_GameplayCard clickedCard, PlayerControls _playerControls)
    {
        if (m_playerControls == null) m_playerControls = _playerControls;
        if (m_currentCard != null) return;

        var _myLogic = GeneralPurposeFunctions.GetPlayerLogicReference();
        if (!_myLogic.TurnActive) return;

        m_currentCard = clickedCard;

        mainRect = m_currentCard.CardSpriteTransform;
        parentToMainRect = mainRect.parent;
        if (mainRect == null)
        {
            Debug.LogWarning("Add Rect Transform to this.");
            return;
        }

        var _canvas = GameObject.FindGameObjectWithTag(GlobalConstantValues.TAG_MAINCANVAS);
        if (_canvas == null)
        {
            Debug.LogWarning("Did you forget to add a MainUICanvas tag? Do you have a Canvas in your scene?");
            return;
        }

        canvasRect = _canvas.GetComponent<RectTransform>();
        mainRect.SetParent(canvasRect);
    }

    private void OnStopClickingCard(InteractionValues _interact)
    {
        var _zone = _interact.TargetZone;
        var dropCard = _interact.DropReason;
        if (m_currentCard == null || _zone == null) return;
        EnumUnitPlacement _placement;
        if (_zone.AllowableCards.Contains(EnumUnitPlacement.Frontline)) _placement = EnumUnitPlacement.Frontline;
        else if (_zone.AllowableCards.Contains(EnumUnitPlacement.Ranged)) _placement = EnumUnitPlacement.Ranged;
        else if (_zone.AllowableCards.Contains(EnumUnitPlacement.Siege)) _placement = EnumUnitPlacement.Siege;
        else
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "You need to check your allowable position on your zones.");
            return;
        }
        
        switch (dropCard)
        {
            case EnumDropCardReason.PlayMinion:
            case EnumDropCardReason.PlaySpy:
            case EnumDropCardReason.PlayGlobal:
                PlayCardPassToServer(_placement);
                break;
            case EnumDropCardReason.PlayDecoy:
                S_GamePlayLogicManager.InteractTarget[] _interactCards = new S_GamePlayLogicManager.InteractTarget[1];
                _interactCards[0] = new S_GamePlayLogicManager.InteractTarget(_interact.DecoyCard.id, _interact.DecotSlot);
                PlayCardPassToServer(_placement, _interactCards);
                break;
        }
    }

    private void OnNotPlayingHeldCard()
    {
        if (m_currentCard == null) return;

        if (parentToMainRect == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "You should be correctly setting card parent to reset card image.");
            return;
        }

        mainRect.SetParent(parentToMainRect);
        mainRect.anchoredPosition = Vector2.zero;
        m_currentCard = null;
    }
}
