using UnityEngine;
using UnityEngine.UI;

public class UI_MulliganButton : UI_MainButtonGame
{
    [SerializeField] private Image m_cardSprite = null;
    [SerializeField] private Button m_cardBtn = null;

    [SerializeField] private Image m_highlight = null;
    private Color _onColor = Color.yellow;
    private Color _offColor = new Color(0f, 0f, 0f, 0f);

    public override Card CardData 
    { 
        get { return m_myData; } 
        set 
        {
            m_myData = value;
            if (m_cardSprite == null) return;
            m_cardSprite.sprite = m_myData.cardImage;
        } 
    }

    private bool _pressed = false;
    public bool IsPressed 
    { 
        get { return _pressed; } 
        set 
        { 
            _pressed = value;
            if (!_pressed) m_highlight.color = _offColor;
            else m_highlight.color = _onColor;
        } 
    }


    private UI_MulliganScroll _mulliganManager = null;
    public void InitializeButton(Card info, int order, UI_MulliganScroll manager)
    {
        if(m_cardSprite == null || m_cardBtn == null || m_highlight == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Mulligan card is missing components.");
            return;
        }

        m_myData = info;
        m_myOrder = order;
        m_cardSprite.sprite = m_myData.cardImage;

        _mulliganManager = manager;
    }

    private void OnEnable()
    {
        if (m_cardSprite == null || m_cardBtn == null) return;

        m_cardBtn.onClick.AddListener(ShowMulliganButton);
    }

    private void OnDisable()
    {
        if (m_cardSprite == null || m_cardBtn == null) return;

        m_cardBtn.onClick.RemoveListener(ShowMulliganButton);
    }

    private void ShowMulliganButton()
    {
        if (_pressed)
        {
            _mulliganManager.HideMulliganCard();
            return;
        }

        _mulliganManager.SendCardToMulligan(m_myData.id, this);
    }
}
