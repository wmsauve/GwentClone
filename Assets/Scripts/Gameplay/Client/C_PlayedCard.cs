using UnityEngine;
using UnityEngine.EventSystems;

public class C_PlayedCard : G_OutlinedGameObject
{

    public override void Start()
    {
        base.Start();
    }

    public void InitializePlayedCard(Card _card, EnumPlayCardReason _condition)
    {
        var _myMaterial = GetComponent<MeshRenderer>();
        if (_myMaterial == null) return;

        Texture2D _sprite = _card.cardImage.texture;

        _myMaterial.material.SetTexture("_MainTex", _sprite);

        _outlineCondition = _condition;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {

    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
