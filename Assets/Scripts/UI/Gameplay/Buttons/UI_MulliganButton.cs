using UnityEngine;
using UnityEngine.UI;

public class UI_MulliganButton : MonoBehaviour
{
    [SerializeField] private Image m_cardSprite = null;
    [SerializeField] private Button m_cardBtn = null;

    [SerializeField] private Image m_highlight = null;
    private Color _onColor = Color.yellow;
    private Color _offColor = new Color(0f, 0f, 0f, 0f);

    private Card _myInfo = null;
    public Card MyCard { get { return _myInfo; } 
        set 
        { 
            _myInfo = value;
            if (m_cardSprite == null) return;
            m_cardSprite.sprite = _myInfo.cardImage;
        } 
    }

    private bool _pressed = false;
    public bool IsPressed { get { return _pressed; } 
        set 
        { 
            _pressed = value;
            if (!_pressed) m_highlight.color = _offColor;
            else m_highlight.color = _onColor;
        } 
    }


    private UI_MulliganScroll _mulliganManager = null;
    public void InitializeButton(Card info, UI_MulliganScroll manager)
    {
        if(m_cardSprite == null || m_cardBtn == null || m_highlight == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Mulligan card is missing components.");
            return;
        }

        _myInfo = info;
        m_cardSprite.sprite = _myInfo.cardImage;

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

        _mulliganManager.SendCardToMulligan(_myInfo.id, this);
    }
}
