using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AgileSelector : MonoBehaviour
{
    [Header("Buttons Related")]
    [SerializeField] private Button m_frontBtn = null;
    [SerializeField] private Button m_rangedBtn = null;
    [SerializeField] private Button m_siegeBtn = null;
    [SerializeField] private Button m_cancel = null;

    public Action<EnumUnitPlacement> OnSelectedAgile;
    public static Action OnCancelSelect;

    // Start is called before the first frame update
    void Start()
    {
        if(m_frontBtn == null || m_siegeBtn == null || m_rangedBtn == null || m_cancel == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "No button components on Agile Selector.");
        }
    }

    private void OnEnable()
    {
        m_frontBtn.onClick.AddListener(() => SetAgilePlacement(EnumUnitPlacement.Frontline));
        m_rangedBtn.onClick.AddListener(() => SetAgilePlacement(EnumUnitPlacement.Ranged));
        m_siegeBtn.onClick.AddListener(() => SetAgilePlacement(EnumUnitPlacement.Siege));

        m_cancel.onClick.AddListener(HideScreen);
    }

    private void OnDisable()
    {
        m_frontBtn.onClick.RemoveListener(() => SetAgilePlacement(EnumUnitPlacement.Frontline));
        m_rangedBtn.onClick.RemoveListener(() => SetAgilePlacement(EnumUnitPlacement.Ranged));
        m_siegeBtn.onClick.RemoveListener(() => SetAgilePlacement(EnumUnitPlacement.Siege));

        m_cancel.onClick.RemoveListener(HideScreen);
    }

    private void SetAgilePlacement(EnumUnitPlacement _placement)
    {
        OnSelectedAgile?.Invoke(_placement);
        gameObject.SetActive(false);
    }

    public void HideScreen()
    {
        gameObject.SetActive(false);
        OnCancelSelect?.Invoke();
    }

    public void ShowScreen(EnumUnitPlacement placement)
    {
        switch (placement)
        {
            case EnumUnitPlacement.Agile_FR:
                m_frontBtn.enabled = true;
                m_rangedBtn.enabled = true;
                m_siegeBtn.enabled = false;
                break;
            case EnumUnitPlacement.Agile_FS:
                m_frontBtn.enabled = true;
                m_rangedBtn.enabled = false;
                m_siegeBtn.enabled = true;
                break;
            case EnumUnitPlacement.Agile_RS:
                m_frontBtn.enabled = false;
                m_rangedBtn.enabled = true;
                m_siegeBtn.enabled = true;
                break;
            default:
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, $"Only Agile. You tried this placement for Agile Selector: {placement}");
                break;
        }

        gameObject.SetActive(true);
    }
}
