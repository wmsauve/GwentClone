using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MulliganScroll : MonoBehaviour
{
    [Header("Main Comp Related")]
    [SerializeField] private GameObject m_mulliganCardPrefab = null;
    [SerializeField] private Transform m_viewTransform = null;
    [SerializeField] private int m_intialHandSize = 10;

    [Header("Scroll Buttons Related")]
    [SerializeField] private Button m_rightBtn = null;
    [SerializeField] private Button m_leftBtn = null;

    [Header("Animation Related")]
    [SerializeField] private List<MulliganSpotParams> _spots = new List<MulliganSpotParams>();

    private List<Anim_MulliganSwap> _cardAnims = new List<Anim_MulliganSwap>();

    private void OnEnable()
    {
        if (m_rightBtn == null || m_leftBtn == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Missing Buttons for Mulligan screen.");
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

        var btnCompR = m_rightBtn.GetComponent<UI_OnButtonHover>();
        if(btnCompR == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Check why your button is missing a component.");
            return;
        }
        btnCompR.InitializeThisUIComp();

        var btnCompL = m_leftBtn.GetComponent<UI_OnButtonHover>();
        if (btnCompL == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Check why your button is missing a component.");
            return;
        }
        btnCompL.InitializeThisUIComp();

        InitializeMulliganCards();
    }

    private void OnDisable()
    {
        m_rightBtn.onClick.RemoveListener(ShiftCardsRight);
        m_leftBtn.onClick.RemoveListener(ShiftCardsLeft);
    }

    private void ShiftCardsRight()
    {
        bool shiftLeftIn = false;

        foreach (Anim_MulliganSwap anim in _cardAnims)
        {
            switch (anim.CurrentPos)
            {
                case EnumMulliganPos.leftout:
                    if (shiftLeftIn) continue;
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
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.rightcenter], EnumMulliganPos.rightcenter);
                    continue;
                case EnumMulliganPos.rightcenter:
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.right], EnumMulliganPos.right);
                    continue;
                case EnumMulliganPos.right:
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
        bool shiftRightIn = false;

        foreach(Anim_MulliganSwap anim in _cardAnims)
        {
            switch (anim.CurrentPos)
            {
                case EnumMulliganPos.leftout:
                    continue;
                case EnumMulliganPos.left:
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.leftout], EnumMulliganPos.leftout);
                    continue;
                case EnumMulliganPos.leftcenter:
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.left], EnumMulliganPos.left);
                    continue;
                case EnumMulliganPos.center:
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.leftcenter], EnumMulliganPos.leftcenter);
                    continue;
                case EnumMulliganPos.rightcenter:
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.center], EnumMulliganPos.center);
                    continue;
                case EnumMulliganPos.right:
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.rightcenter], EnumMulliganPos.rightcenter);
                    continue;
                case EnumMulliganPos.rightout:
                    if (shiftRightIn) continue;
                    anim.BeginThisAnimation(_spots[(int)EnumMulliganPos.right], EnumMulliganPos.right);
                    shiftRightIn = true;
                    continue;
                default:
                    GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Find out why you are shifting Mulligan cards to invalid spot.");
                    return;
            }
        }
    }

    private void InitializeMulliganCards()
    {
        for(int i = 0; i < m_intialHandSize; i++)
        {
            var newCard = Instantiate(m_mulliganCardPrefab, m_viewTransform);
            var animComp = newCard.GetComponent<Anim_MulliganSwap>();

            if(animComp == null)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Missing Anim component on Mulligan card.");
                return;
            }

            _cardAnims.Add(animComp);
            if (i == 0) animComp.InitializeMullgianCard(_spots[(int)EnumMulliganPos.center], EnumMulliganPos.center);
            else if (i == 1) animComp.InitializeMullgianCard(_spots[(int)EnumMulliganPos.rightcenter], EnumMulliganPos.rightcenter);
            else if (i == 2) animComp.InitializeMullgianCard(_spots[(int)EnumMulliganPos.right], EnumMulliganPos.right);
            else animComp.InitializeMullgianCard(_spots[(int)EnumMulliganPos.rightout], EnumMulliganPos.rightout);
        }
    }

}
