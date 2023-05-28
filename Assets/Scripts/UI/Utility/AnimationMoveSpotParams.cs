using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationMoveSpotParams : MonoBehaviour
{
    [Header("Related Transform Anim")]
    [SerializeField] private float m_cardScale = 1f;
    [SerializeField] private float m_cardPosY = 1f;
    [SerializeField] private float m_cardPosX = 1f;

    [Header("Different Parent Related")]
    [SerializeField] private bool m_useThisScale = false;
    [SerializeField] private bool m_useThisTransformX = false;
    [SerializeField] private bool m_useThisTransformY = false;

    private RectTransform m_transform;

    private void Start()
    {
        m_transform = GetComponent<RectTransform>();
        if(m_transform == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You can't use this without its RectTransform.");
            return;
        }
    }

    public float CardScale {  
        get 
        {
            if (m_useThisScale && m_transform != null)
            {
                return m_transform.localScale.x;
            }
            return m_cardScale; 
        } 
    }
    public float CardPosY { 
        get 
        {
            if (m_useThisTransformY && m_transform != null)
            {
                return m_transform.position.y;
            }
            return m_cardPosY; 
        } 
    }
    public float CardPosX { 
        get 
        {
            if (m_useThisTransformX && m_transform != null)
            {
                return m_transform.position.x;
            }
            return m_cardPosX; 
        } 
    }
}
