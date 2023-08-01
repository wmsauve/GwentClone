using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_OutlinedGameObject : UI_HoverOverButton
{
    private Outline m_myOutline;
    [Header("Outline Related")]
    [SerializeField] private float m_outlineWidth = 5f;
    //[SerializeField] protected EnumPlayCardReason _outlineCondition;

    private EnumPlayerControlsStatus _currentClickMode;

    public virtual void Start()
    {
        m_myOutline = GetComponent<Outline>();
        if (m_myOutline == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Your Zone doesnt have an Outliner.");
            return;
        }
        m_myOutline.OutlineWidth = m_outlineWidth;
        m_myOutline.enabled = false;
    }

    private void OnEnable()
    {
        GlobalActions.OnClickModeChange += SetNewClickMode;
    }

    private void OnDisable()
    {
        GlobalActions.OnClickModeChange -= SetNewClickMode;
    }

    private void SetNewClickMode(EnumPlayerControlsStatus newMode)
    {
        _currentClickMode = newMode;
    }

    public void ShowOutline()
    {
        //if (_currentClickMode != _outlineCondition) return;

        m_myOutline.enabled = true;
    }
    public void HideOutline()
    {
        m_myOutline.enabled = false;
    }
}
