using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_HoverTooltip : MonoBehaviour
{
    [Header("Main Related")]
    [SerializeField] private GameObject m_leaderUI = null;
    [SerializeField] private GameObject m_cardUI = null;
    [SerializeField] private Image m_objectSprite = null;

    [Header("Leader Related")]
    [SerializeField] private TextMeshProUGUI m_leaderName = null;
    [SerializeField] private TextMeshProUGUI m_leaderAbility = null;

    [Header("Card Related")]
    [SerializeField] private TextMeshProUGUI m_cardName = null;
    [SerializeField] private TextMeshProUGUI m_cardAbility = null;
    [SerializeField] private TextMeshProUGUI m_cardPower = null;

    private RectTransform canvasRect = null;
    private RectTransform mainRect = null;

    void Start()
    {
        if(m_leaderUI == null || m_cardUI == null || m_objectSprite == null)
        {
            Debug.LogWarning("Your tooltip doesnt have the references for the different UIs");
            return;
        }

        if(m_leaderName == null || m_leaderAbility == null)
        {
            Debug.LogWarning("You don't have leader text fields added for the tooltip.");
            return;
        }

        mainRect = GetComponent<RectTransform>();
        if(mainRect == null)
        {
            Debug.LogWarning("Add Rect Transform to this.");
            return;
        }

        var _canvas = GameObject.FindGameObjectWithTag(GlobalConstantValues.TAG_MAINCANVAS);
        if (_canvas == null)
        {
            Debug.LogWarning("Did you forget to add a MainUICanvas tag? Do you have a Canvas in your scene?");
            return;
        }

        canvasRect = _canvas.GetComponent<RectTransform>();

    }

    void Update()
    {
        if (canvasRect == null || mainRect == null) return;
        Vector2 anchoredPos = Input.mousePosition / canvasRect.localScale.x;

        if(anchoredPos.x + mainRect.rect.width > canvasRect.rect.width)
        {
            anchoredPos.x = canvasRect.rect.width - mainRect.rect.width;
        }
        if (anchoredPos.y + mainRect.rect.height > canvasRect.rect.height)
        {
            anchoredPos.y = canvasRect.rect.height - mainRect.rect.height;
        }

        mainRect.anchoredPosition = anchoredPos;
    }

    public void PassInfoToTooltip<T>(T info)
    {
        if(typeof(T) == typeof(Card))
        {
            var cardInfo = info as Card;
            m_leaderUI.SetActive(false);
            m_cardUI.SetActive(true);
            m_objectSprite.sprite = cardInfo.cardImage;
            m_cardName.text = cardInfo.id;

            string _effects = "";
            foreach(EnumCardEffects effect in cardInfo.cardEffects)
            {
                if(effect == EnumCardEffects.Scorch || effect == EnumCardEffects.Weather)
                {
                    _effects += " " + GeneralPurposeFunctions.ReturnSkillDescription(effect, cardInfo.unitPlacement);
                    continue;
                }
                _effects += " " + GeneralPurposeFunctions.ReturnSkillDescription(effect);
            }

            m_cardAbility.text = _effects;
            m_cardPower.text = "Card Power: " + cardInfo.cardPower.ToString();

        }
        else if (typeof(T) == typeof(Leader))
        {
            var leaderInfo = info as Leader;
            m_leaderUI.SetActive(true);
            m_cardUI.SetActive(false);
            m_objectSprite.sprite = leaderInfo.cardImage;
            m_leaderName.text = leaderInfo.id;
            m_leaderAbility.text = leaderInfo.abilityDescription;
        }
        else
        {
            Debug.Log("Not a type this method cares about.");
            return;
        }
    }
}
