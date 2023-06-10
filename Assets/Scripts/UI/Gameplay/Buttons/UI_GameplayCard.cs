using UnityEngine;
using UnityEngine.UI;

public class UI_GameplayCard : UI_MainButtonGame
{
    [SerializeField] private Canvas m_canvasComp;
    [SerializeField] private Image m_cardSprite;
    [SerializeField] private Anim_TransformUI m_anim;

    public Anim_TransformUI Anim { get { return m_anim; } }

    private int m_cacheFirstSortOrder = 0;
    private int m_mySortOrder = 0;
    public override int CardOrder
    { 
        get { return m_mySortOrder; }
        set
        {
            m_mySortOrder = value;
            if(m_canvasComp != null)
            {
                m_canvasComp.sortingOrder = value;
            }
        }
    }

    public override Card CardData 
    { 
        get { return m_myData; } 
        set 
        { 
            m_myData = value;
            m_cardSprite.sprite = m_myData.cardImage;
        } 
    }

    public void InitializeCardComponent(int _newOrder, float _raycastWidth)
    {
        m_cacheFirstSortOrder = _newOrder;

        var _rectComp = GetComponent<RectTransform>();
        if(_rectComp == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Need Rect Transform on this button.");
            return;
        }

        Vector2 _sizeDelta = new Vector2(-(_rectComp.rect.width - _rectComp.rect.width * _raycastWidth), 0f);
        _rectComp.sizeDelta = _sizeDelta;
        var _parentRect = transform.parent.GetComponent<RectTransform>();
        if(_parentRect == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Need Rect Transform on this object.");
            return;
        }
        var _leftPos = -(_parentRect.rect.width) / 2f + _rectComp.rect.width / 2f;

        _rectComp.localPosition = new Vector3(_leftPos, _rectComp.localPosition.y);
    }

    private void Start()
    {
        if(m_canvasComp == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You need canvas comp on your gameplay card buttons.");
            return;
        }

        if(m_cardSprite == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You need an Image comp on your gameplay card buttons.");
            return;
        }
    }

    public void ResetSortOrder()
    {
        CardOrder = m_cacheFirstSortOrder;
    }

    public void SetNewlyAdjustedPositions(int newPos)
    {
        m_cacheFirstSortOrder = newPos;
        CardOrder = newPos;
    }
}
