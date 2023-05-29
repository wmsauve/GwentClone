using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_Controller : MonoBehaviour
{
    protected Animator m_animator;

    protected virtual void Start()
    {
        m_animator = GetComponent<Animator>();
        if(m_animator == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Your controller doesn't have an animator.");
            return;
        }


    }

    protected virtual void Update()
    {

    }
}
