using UnityEngine;
using UnityEngine.EventSystems;

public class C_PlayedCard : G_OutlinedGameObject
{

    private Card m_myCard;
    public Card MyCard { get { return m_myCard; } }

    public override void Start()
    {
        base.Start();
    }

    public void InitializePlayedCard(Card _card)
    {
        m_myCard = _card;

        var _myMaterial = GetComponent<MeshRenderer>();
        if (_myMaterial == null) return;

        Texture2D _sprite = _card.cardImage.texture;

        _myMaterial.material.SetTexture("_MainTex", _sprite);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {

    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
