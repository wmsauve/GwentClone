using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_MulliganScroll : MonoBehaviour
{
    [Header("Main Comp Related")]
    [SerializeField] private GameObject m_mulliganCardPrefab = null;
    [SerializeField] private TextMeshProUGUI m_mulliganCounter = null;
    [SerializeField] private Transform m_viewTransform = null;
    [SerializeField] private int m_intialHandSize = GlobalConstantValues.GAME_INITIALHANDSIZE;

    [Header("Scroll Buttons Related")]
    [SerializeField] private Button m_rightBtn = null;
    [SerializeField] private Button m_leftBtn = null;
    [SerializeField] private Button m_mulliganBtn = null;

    [Header("Animation Related")]
    [SerializeField] private List<MulliganSpotParams> _spots = new List<MulliganSpotParams>();

    private List<Anim_MulliganSwap> _cardAnims = new List<Anim_MulliganSwap>();
    private List<UI_MulliganButton> _buttons = new List<UI_MulliganButton>();
    private S_GamePlayLogicManager _gameManager;
    private float _mulliganAnimDuration;
    private float _cooldownBtnPress;
    private bool _animRunning = false;
    private string _cardToMulligan;

    private int _rightMostIndex;
    private int _leftMostIndex;

    private void Update()
    {
        if (!_animRunning) return;
        _cooldownBtnPress += Time.deltaTime;
        if(_cooldownBtnPress > _mulliganAnimDuration) _animRunning = false;
    }

    private void OnEnable()
    {
        if (m_rightBtn == null || m_leftBtn == null || m_mulliganBtn == null || m_mulliganCounter == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Missing Components for Mulligan screen.");
            return;
        }

        if (_spots.Count <= 0)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Missing Mulligan position references.");
            return;
        }

        if(m_mulliganCardPrefab == null || m_viewTransform == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Can't spawn mulligan cards.");
            return;
        }

        m_rightBtn.onClick.AddListener(ShiftCardsRight);
        m_leftBtn.onClick.AddListener(ShiftCardsLeft);
        m_mulliganBtn.onClick.AddListener(MulliganCard);

        var btnCompR = m_rightBtn.GetComponent<UI_OnButtonHover>();
        var btnCompL = m_leftBtn.GetComponent<UI_OnButtonHover>();
        var btnCompM = m_mulliganBtn.GetComponent<UI_OnButtonHover>();
        if (btnCompR == null || btnCompL == null || btnCompM == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Check why your button is missing a component.");
            return;
        }

        btnCompR.InitializeThisUIComp();
        btnCompL.InitializeThisUIComp();
        btnCompM.InitializeThisUIComp();
        m_mulliganBtn.gameObject.SetActive(false);

    }

    private void OnDisable()
    {
        m_rightBtn.onClick.RemoveListener(ShiftCardsRight);
        m_leftBtn.onClick.RemoveListener(ShiftCardsLeft);
        m_mulliganBtn.onClick.RemoveListener(MulliganCard);
    }

    private void ShiftCardsRight()
    {
        if (!RunCheckForValidShift(m_rightBtn)) return;
        HideMulliganCard();
        if (!_animRunning)
        {
            _animRunning = true;
            _cooldownBtnPress = 0f;
        }


        bool shiftLeftIn = false;
       
        foreach (Anim_MulliganSwap anim in _cardAnims)
        {
            switch (anim.CurrentPos)
            {
                case EnumMulliganPos.leftout:
                    if (shiftLeftIn) continue;

                    var myIndex = _cardAnims.IndexOf(anim);
                    if (myIndex != _leftMostIndex - 1) continue;
                    _leftMostIndex = myIndex;

                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.left], EnumMulliganPos.left);
                    shiftLeftIn = true;
                    continue;
                case EnumMulliganPos.left:
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.leftcenter], EnumMulliganPos.leftcenter);
                    continue;
                case EnumMulliganPos.leftcenter:
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.center], EnumMulliganPos.center);
                    continue;
                case EnumMulliganPos.center:
                    _mulliganAnimDuration = anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.rightcenter], EnumMulliganPos.rightcenter);
                    continue;
                case EnumMulliganPos.rightcenter:
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.right], EnumMulliganPos.right);
                    continue;
                case EnumMulliganPos.right:
                    _rightMostIndex -= 1;
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.rightout], EnumMulliganPos.rightout);
                    continue;
                case EnumMulliganPos.rightout:
                    continue;
                default:
                    GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Find out why you are shifting Mulligan cards to invalid spot.");
                    return;
            }
        }
    }

    private void ShiftCardsLeft()
    {
        if (!RunCheckForValidShift(m_leftBtn)) return;
        HideMulliganCard();
        if (!_animRunning)
        {
            _animRunning = true;
            _cooldownBtnPress = 0f;
        }

        bool shiftRightIn = false;

        foreach(Anim_MulliganSwap anim in _cardAnims)
        {
            switch (anim.CurrentPos)
            {
                case EnumMulliganPos.leftout:
                    continue;
                case EnumMulliganPos.left:
                    _leftMostIndex += 1;
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.leftout], EnumMulliganPos.leftout);
                    continue;
                case EnumMulliganPos.leftcenter:
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.left], EnumMulliganPos.left);
                    continue;
                case EnumMulliganPos.center:
                    _mulliganAnimDuration = anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.leftcenter], EnumMulliganPos.leftcenter);
                    continue;
                case EnumMulliganPos.rightcenter:
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.center], EnumMulliganPos.center);
                    continue;
                case EnumMulliganPos.right:
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.rightcenter], EnumMulliganPos.rightcenter);
                    continue;
                case EnumMulliganPos.rightout:
                    if (shiftRightIn) continue;

                    var myIndex = _cardAnims.IndexOf(anim);
                    if (myIndex != _rightMostIndex + 1) continue;
                    _rightMostIndex = myIndex;

                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.right], EnumMulliganPos.right);
                    shiftRightIn = true;
                    continue;
                default:
                    GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Find out why you are shifting Mulligan cards to invalid spot.");
                    return;
            }
        }
    }

    public void InitializeMulliganCards(List<Card> cardInfo, S_GamePlayLogicManager manager, int startMulligans)
    {
        _gameManager = manager;

        for(int i = 0; i < m_intialHandSize; i++)
        {
            var newCard = Instantiate(m_mulliganCardPrefab, m_viewTransform);
            var animComp = newCard.GetComponentInChildren<Anim_MulliganSwap>();
            var buttonComp = newCard.GetComponentInChildren<UI_MulliganButton>();
            
            if(animComp == null || buttonComp == null)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Missing components on Mulligan card.");
                return;
            }

            buttonComp.InitializeButton(cardInfo[i], this);

            _cardAnims.Add(animComp);
            _buttons.Add(buttonComp);
            if (i == 0) animComp.InitializeMullgianCard(_spots[(int)EnumMulliganPos.center], EnumMulliganPos.center);
            else if (i == 1) animComp.InitializeMullgianCard(_spots[(int)EnumMulliganPos.rightcenter], EnumMulliganPos.rightcenter);
            else if (i == 2) animComp.InitializeMullgianCard(_spots[(int)EnumMulliganPos.right], EnumMulliganPos.right);
            else animComp.InitializeMullgianCard(_spots[(int)EnumMulliganPos.rightout], EnumMulliganPos.rightout);
        }

        _leftMostIndex = 0;
        _rightMostIndex = 2;
        UpdateMulligansText(startMulligans);
    }

    private bool RunCheckForValidShift(Button dir)
    {
        if (dir == m_rightBtn)
        {
            foreach(Anim_MulliganSwap anim in _cardAnims)
            {
                if (anim.CurrentPos == EnumMulliganPos.leftcenter) return true;
            }

            return false;
        }

        if (dir == m_leftBtn)
        {
            foreach (Anim_MulliganSwap anim in _cardAnims)
            {
                if (anim.CurrentPos == EnumMulliganPos.rightcenter) return true;
            }

            return false;
        }

        return false;
    }

    public void UpdateMulliganedButton(Card newCard, int mulliganCount)
    {
        foreach(UI_MulliganButton button in _buttons)
        {
            if (button.IsPressed)
            {
                button.IsPressed = false;
                button.MyCard = newCard;
                break;
            }
        }

        UpdateMulligansText(mulliganCount);
    }

    private void UpdateMulligansText(int mulliganCount)
    {
        m_mulliganCounter.text = "Mulligans: " + mulliganCount;
    }

    public void HideMulliganCard()
    {
        _cardToMulligan = string.Empty;
        m_mulliganBtn.gameObject.SetActive(false);
        foreach (UI_MulliganButton button in _buttons)
        {
            if (button.IsPressed) button.IsPressed = false;
        }
    }

    private void MulliganCard()
    {
        _gameManager.MulliganACardServerRpc(_cardToMulligan);
    }

    public void SendCardToMulligan(string cardName, UI_MulliganButton pressed)
    {
        foreach (UI_MulliganButton button in _buttons)
        {
            if (button.IsPressed)
            {
                button.IsPressed = false;
                break;
            }
        }
        pressed.IsPressed = true;
        m_mulliganBtn.gameObject.SetActive(true);
        _cardToMulligan = cardName;
    }

    public void FinishMulligan()
    {

    }

}
