using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_CardViewScroll : MonoBehaviour
{
    [Header("Main Comp Related")]
    [SerializeField] protected GameObject m_cardPrefab = null;
    [SerializeField] protected Transform m_viewTransform = null;

    [Header("Scroll Buttons Related")]
    [SerializeField] protected Button m_rightBtn = null;
    [SerializeField] protected Button m_leftBtn = null;
    
    [Header("Animation Related")]
    [SerializeField] protected List<AnimationMoveSpotParams> _spots = new List<AnimationMoveSpotParams>();

    protected List<Anim_TransformUI> _cardAnims = new List<Anim_TransformUI>();
    protected List<UI_ScrollCardButton> _buttons = new List<UI_ScrollCardButton>();
    protected S_GamePlayLogicManager _gameManager;
    protected float _animDuration;
    protected float _cooldownBtnPress;
    protected bool _animRunning = false;

    //Send to Server Related
    protected string _cardToSelect;
    protected string _cardUnique;
    protected int _cardSlot;

    protected int _rightMostIndex;
    protected int _leftMostIndex;

    private void Update()
    {
        if (!_animRunning) return;
        _cooldownBtnPress += Time.deltaTime;
        if(_cooldownBtnPress > _animDuration) _animRunning = false;
    }

    protected virtual void Start()
    {
        _gameManager = GeneralPurposeFunctions.GetComponentFromScene<S_GamePlayLogicManager>();
        if(_gameManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Can't find Game Manager");
            return;
        }

        gameObject.SetActive(false);
        _leftMostIndex = 0;
        _rightMostIndex = 2;
    }

    protected virtual void OnEnable()
    {
        if (m_rightBtn == null || m_leftBtn == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Missing Components for card scroll screen.");
            return;
        }

        if (_spots.Count <= 0)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Missing card scroll position references.");
            return;
        }

        if(m_cardPrefab == null || m_viewTransform == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Can't spawn scroll cards.");
            return;
        }

        m_rightBtn.onClick.AddListener(ShiftCardsRight);
        m_leftBtn.onClick.AddListener(ShiftCardsLeft);
        

        var btnCompR = m_rightBtn.GetComponent<UI_OnButtonHover>();
        var btnCompL = m_leftBtn.GetComponent<UI_OnButtonHover>();
        if (btnCompR == null || btnCompL == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Check why your button is missing a component.");
            return;
        }

        btnCompR.InitializeThisUIComp();
        btnCompL.InitializeThisUIComp();
    }

    protected virtual void OnDisable()
    {
        m_rightBtn.onClick.RemoveListener(ShiftCardsRight);
        m_leftBtn.onClick.RemoveListener(ShiftCardsLeft);
    }

    protected virtual void ShiftCardsRight()
    {
        if (!RunCheckForValidShift(m_rightBtn)) return;
        HideCard();
        if (!_animRunning)
        {
            _animRunning = true;
            _cooldownBtnPress = 0f;
        }

        bool shiftLeftIn = false;
       
        foreach (Anim_TransformUI anim in _cardAnims)
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
                    _animDuration = anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.rightcenter], EnumMulliganPos.rightcenter);
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

    protected virtual void ShiftCardsLeft()
    {
        if (!RunCheckForValidShift(m_leftBtn)) return;
        HideCard();
        if (!_animRunning)
        {
            _animRunning = true;
            _cooldownBtnPress = 0f;
        }

        bool shiftRightIn = false;

        foreach(Anim_TransformUI anim in _cardAnims)
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
                    _animDuration = anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.leftcenter], EnumMulliganPos.leftcenter);
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

    protected bool RunCheckForValidShift(Button dir)
    {
        if (dir == m_rightBtn)
        {
            foreach(Anim_TransformUI anim in _cardAnims)
            {
                if (anim.CurrentPos == EnumMulliganPos.leftcenter) return true;
            }

            return false;
        }

        if (dir == m_leftBtn)
        {
            foreach (Anim_TransformUI anim in _cardAnims)
            {
                if (anim.CurrentPos == EnumMulliganPos.rightcenter) return true;
            }

            return false;
        }

        return false;
    }

    private void HideCard()
    {
        _cardToSelect = string.Empty;
        _cardSlot = -1;
        foreach (UI_ScrollCardButton button in _buttons)
        {
            if (button.IsPressed) button.IsPressed = false;
        }
    }

    public virtual void InteractWithScrollCard(string cardName, string unique, UI_ScrollCardButton pressed)
    {
        foreach (UI_ScrollCardButton button in _buttons)
        {
            if (button.IsPressed)
            {
                button.IsPressed = false;
                break;
            }
        }
        pressed.IsPressed = true;
        _cardToSelect = cardName;
        _cardUnique = unique;
        _cardSlot = pressed.CardOrder;
    }
}
