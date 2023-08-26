using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UI_MulliganCards : UI_CardViewScroll
{
    [Header("Mulligan Related")]
    [SerializeField] private GameObject m_waitingMessage = null;
    [SerializeField] private TextMeshProUGUI m_mulliganCounter = null;
    [SerializeField] private int m_intialHandSize = GlobalConstantValues.GAME_INITIALHANDSIZE;
    [SerializeField] private Button m_mulliganBtn = null;

    protected override void Start()
    {
        base.Start();
        if (m_waitingMessage == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "No waiting message for ending mulligan.");
            return;
        }

        if (m_waitingMessage.activeSelf) m_waitingMessage.SetActive(false);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if(m_mulliganBtn == null || m_mulliganCounter == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Missing Components for mulligan scroll screen.");
            return;
        }

        m_mulliganBtn.onClick.AddListener(MulliganCard);

        var btnCompM = m_mulliganBtn.GetComponent<UI_OnButtonHover>();
        if (btnCompM == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Check why your button is missing a component.");
            return;
        }
        btnCompM.InitializeThisUIComp();
        m_mulliganBtn.gameObject.SetActive(false);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        m_mulliganBtn.onClick.RemoveListener(MulliganCard);
    }

    protected override void ShiftCardsLeft()
    {
        base.ShiftCardsLeft();
        if (!RunCheckForValidShift(m_leftBtn)) return;
        HideMulliganButton();
    }

    protected override void ShiftCardsRight()
    {
        base.ShiftCardsRight();
        if (!RunCheckForValidShift(m_rightBtn)) return;
        HideMulliganButton();
    }

    public void InitializeMulliganCards(List<Card> cardInfo, int startMulligans)
    {
        for (int i = 0; i < m_intialHandSize; i++)
        {
            var newCard = Instantiate(m_cardPrefab, m_viewTransform);
            var animComp = newCard.GetComponentInChildren<Anim_TransformUI>();
            var buttonComp = newCard.GetComponentInChildren<UI_ScrollCardButton>();

            if (animComp == null || buttonComp == null)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Missing components on Mulligan card.");
                return;
            }

            buttonComp.InitializeButton(cardInfo[i], i, this);

            _cardAnims.Add(animComp);
            _buttons.Add(buttonComp);
            if (i == 0) animComp.InitializeCardForAnim(_spots[(int)EnumMulliganPos.center], EnumMulliganPos.center);
            else if (i == 1) animComp.InitializeCardForAnim(_spots[(int)EnumMulliganPos.rightcenter], EnumMulliganPos.rightcenter);
            else if (i == 2) animComp.InitializeCardForAnim(_spots[(int)EnumMulliganPos.right], EnumMulliganPos.right);
            else animComp.InitializeCardForAnim(_spots[(int)EnumMulliganPos.rightout], EnumMulliganPos.rightout);
        }

        UpdateMulligansText(startMulligans);
    }

    public void UpdateMulliganedButton(Card newCard, int mulliganCount)
    {
        foreach (UI_ScrollCardButton button in _buttons)
        {
            if (button.IsPressed)
            {
                button.IsPressed = false;
                button.CardData = newCard;
                break;
            }
        }

        if (mulliganCount == 0)
        {
            m_waitingMessage.SetActive(true);
            gameObject.SetActive(false);
            return;
        }
        UpdateMulligansText(mulliganCount);
    }

    private void UpdateMulligansText(int mulliganCount)
    {
        m_mulliganCounter.text = "Mulligans: " + mulliganCount;
    }

    private void MulliganCard()
    {
        _gameManager.MulliganACardServerRpc(_cardToSelect, _cardSlot);
    }

    public override void InteractWithScrollCard(string cardName, UI_ScrollCardButton pressed)
    {
        base.InteractWithScrollCard(cardName, pressed);
        m_mulliganBtn.gameObject.SetActive(true);
    }

    public void HideMulliganButton()
    {
        m_mulliganBtn.gameObject.SetActive(false);
    }
}
