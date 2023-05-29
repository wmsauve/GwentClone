using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_NpcController : Anim_Controller
{
    [SerializeField] private float m_minRandomAmount = 0f;
    [SerializeField] private float m_maxRandomAmount = 1f;
    [SerializeField] private string[] m_uniqueAnims;

    private bool _beginAnims = false;
    private float _timer = 0f;
    private float _counter = 0;
    private int _animToPlay = 0;

    protected override void Start()
    {
        base.Start();

        if(m_minRandomAmount > m_maxRandomAmount)
        {
            m_minRandomAmount = m_maxRandomAmount;
        }

        _timer = Random.Range(m_minRandomAmount, m_maxRandomAmount);
        _counter = 0f;
        _beginAnims = true;
        _animToPlay = Random.Range(0, m_uniqueAnims.Length - 1);
    }


    protected override void Update()
    {
        if (!_beginAnims) return;

        _counter += Time.deltaTime;
        if (_counter >= _timer)
        {
            _counter = 0f;
            _timer = Random.Range(m_minRandomAmount, m_maxRandomAmount);
            m_animator.SetTrigger(m_uniqueAnims[_animToPlay]);
            _animToPlay = Random.Range(0, m_uniqueAnims.Length - 1);
        }
    }

}
