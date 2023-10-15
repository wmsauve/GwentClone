using UnityEngine;
using UnityEngine.UI;

public class UI_ScrollCardButton : UI_MainButtonGame
{
    [SerializeField] protected Image m_cardSprite = null;
    [SerializeField] protected Button m_cardBtn = null;

    [SerializeField] private Image m_highlight = null;
    private Color _onColor = Color.yellow;
    private Color _offColor = new Color(0f, 0f, 0f, 0f);

    public override GwentCard CardData 
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


    protected UI_CardViewScroll _manager = null;
    public virtual void InitializeButton(GwentCard info, int order, UI_CardViewScroll manager)
    {
        if(m_cardSprite == null || m_cardBtn == null || m_highlight == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Mulligan card is missing components.");
            return;
        }

        m_myData = info;
        m_myOrder = order;
        m_cardSprite.sprite = m_myData.cardImage;

        _manager = manager;
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }


}
