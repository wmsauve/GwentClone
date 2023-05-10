using UnityEngine;
using UnityEngine.UI;

public class UI_MulliganButton : MonoBehaviour
{
    [SerializeField] private Image m_cardSprite = null;
    [SerializeField] private Button m_cardBtn = null;

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
    public bool IsPressed { get { return _pressed; } set { _pressed = value; } }


    private S_GamePlayLogicManager serverRpcManager = null;
    public void InitializeButton(Card info, S_GamePlayLogicManager manager)
    {
        if(m_cardSprite == null || m_cardBtn == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Mulligan card is missing components.");
            return;
        }

        _myInfo = info;
        m_cardSprite.sprite = _myInfo.cardImage;

        serverRpcManager = manager;
    }

    private void OnEnable()
    {
        if (m_cardSprite == null || m_cardBtn == null) return;

        m_cardBtn.onClick.AddListener(MulliganTheCard);
    }

    private void OnDisable()
    {
        if (m_cardSprite == null || m_cardBtn == null) return;

        m_cardBtn.onClick.RemoveListener(MulliganTheCard);
    }

    private void MulliganTheCard()
    {
        _pressed = true;
        serverRpcManager.MulliganACardServerRpc(_myInfo.id);
    }
}
