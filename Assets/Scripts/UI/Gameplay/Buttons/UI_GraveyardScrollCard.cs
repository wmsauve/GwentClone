using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GraveyardScrollCard : UI_ScrollCardButton
{
    private UI_GraveyardCards graveyardManager = null;

    public override void InitializeButton(Card info, int order, UI_CardViewScroll manager)
    {
        base.InitializeButton(info, order, manager);

        if (_manager is UI_GraveyardCards) graveyardManager = (UI_GraveyardCards)_manager;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (m_cardSprite == null || m_cardBtn == null) return;

        m_cardBtn.onClick.AddListener(InteractWithGraveyardButton);
        AgileSelector.OnCancelSelect += AgileNotSelected;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (m_cardSprite == null || m_cardBtn == null) return;

        m_cardBtn.onClick.RemoveListener(InteractWithGraveyardButton);
        AgileSelector.OnCancelSelect += AgileNotSelected;
    }

    private void InteractWithGraveyardButton()
    {
        if (IsPressed)
        {
            graveyardManager.HideInteractButton();
            return;
        }
        
        graveyardManager.InteractWithScrollCard(m_myData.id, this);
    }

    private void AgileNotSelected()
    {
        if (!IsPressed) return;

        IsPressed = false;
    }
}
