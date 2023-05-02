using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class UI_OnButtonHover : UI_InitializeFromManager, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Objects Related")]
    [SerializeField] private GameObject m_borderElement = null;

    [Header("Parameters Related")]
    [SerializeField] private string m_text = "";

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_borderElement == null) return;

        var _borderImage = m_borderElement.GetComponent<Image>();
        if (_borderImage == null) return;
        _borderImage.enabled = true;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_borderElement == null) return;

        var _borderImage = m_borderElement.GetComponent<Image>();
        if (_borderImage == null) return;
        _borderImage.enabled = false;

    }

    public override void InitializeThisUIComp()
    {
        var _text = transform.GetComponentInChildren<TextMeshProUGUI>();
        if (_text == null) return;
        if (m_borderElement == null) return;
        _text.text = m_text;
        var _borderImage = m_borderElement.GetComponent<Image>();
        if (_borderImage == null) return;
        _borderImage.enabled = false;

    }

}
