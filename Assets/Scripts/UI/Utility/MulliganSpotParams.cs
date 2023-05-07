using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MulliganSpotParams : MonoBehaviour
{
    [Header("Related To Mulligan Anim")]
    [SerializeField] private float m_cardScale = 1f;
    [SerializeField] private float m_cardPosY = 1f;
    [SerializeField] private float m_cardPosX = 1f;

    public float CardScale { get { return m_cardScale; } }
    public float CardPosY { get { return m_cardPosY; } }
    public float CardPosX { get { return m_cardPosX; } }
}
