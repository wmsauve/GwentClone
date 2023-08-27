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

    }

    private void OnDisable()
    {
        m_frontBtn.onClick.RemoveListener(() => SetAgilePlacement(EnumUnitPlacement.Frontline));
        m_rangedBtn.onClick.RemoveListener(() => SetAgilePlacement(EnumUnitPlacement.Ranged));
        m_siegeBtn.onClick.RemoveListener(() => SetAgilePlacement(EnumUnitPlacement.Siege));
    }

    private void SetAgilePlacement(EnumUnitPlacement _placement)
    {
        OnSelectedAgile?.Invoke(_placement);
    }

    public void HideScreen()
    {
        gameObject.SetActive(false);
    }

    public void ShowScreen()
    {
        gameObject.SetActive(true);
    }
}
