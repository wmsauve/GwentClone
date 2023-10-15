using UnityEngine;
using UnityEngine.EventSystems;

public class C_PlayedCard : G_OutlinedGameObject
{

    private GwentCard m_myCard;
    public GwentCard MyCard { 
        get { return m_myCard; }
        set
        {
            m_myCard = value;
            SetMaterial();
        }
    }

    private C_GameZone m_myZone;
    public C_GameZone MyZone { get { return m_myZone; } }

    public override void Start()
    {
        base.Start();
    }

    public void InitializePlayedCard(GwentCard _card, C_GameZone _zone)
    {
        m_myCard = _card;
        m_myZone = _zone;
        SetMaterial();
    }

    public int GetMyLocation()
    {
        return gameObject.transform.GetSiblingIndex();
    }

    private void SetMaterial()
    {
        var _myMaterial = GetComponent<MeshRenderer>();
        if (_myMaterial == null) return;

        Texture2D _sprite = m_myCard.cardImage.texture;

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
