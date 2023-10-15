using UnityEngine;

public class UI_MulliganScrollButton : UI_ScrollCardButton
{
    private UI_MulliganCards mulliganManager = null;

    public override void InitializeButton(GwentCard info, int order, UI_CardViewScroll manager)
    {
        base.InitializeButton(info, order, manager);

        if (_manager is UI_MulliganCards) mulliganManager = (UI_MulliganCards)_manager;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (m_cardSprite == null || m_cardBtn == null) return;

        m_cardBtn.onClick.AddListener(ShowMulliganButton);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (m_cardSprite == null || m_cardBtn == null) return;

        m_cardBtn.onClick.RemoveListener(ShowMulliganButton);

    }

    private void ShowMulliganButton()
    {
        if (IsPressed)
        {
            mulliganManager.HideMulliganButton();
            return;
        }

        mulliganManager.InteractWithScrollCard(m_myData.id, m_myData.UniqueGuid, this);
    }
}
