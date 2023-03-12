using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class UI_InitializeFromManager : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            GlobalActions.OnInitializeAllUI += InitializeThisUIComp;
        }

        protected virtual void OnDisable()
        {
            GlobalActions.OnInitializeAllUI -= InitializeThisUIComp;
        }

        protected virtual void InitializeThisUIComp()
        {

        }
    }

}

