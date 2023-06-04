using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_PlayedCard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializePlayedCard(Card _card)
    {
        var _myMaterial = GetComponent<MeshRenderer>();
        if (_myMaterial == null) return;

        Texture2D _sprite = _card.cardImage.texture;

        _myMaterial.material.SetTexture("_MainTex", _sprite);
    }
}
