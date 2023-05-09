using UnityEngine;
using UnityEngine.UI;

public class UI_MulliganButton : MonoBehaviour
{
    [SerializeField] private Image m_cardSprite = null;
    [SerializeField] private Button m_cardBtn = null;

    private Card _myInfo = null;
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
        serverRpcManager.MulliganACardServerRpc();
    }
}
