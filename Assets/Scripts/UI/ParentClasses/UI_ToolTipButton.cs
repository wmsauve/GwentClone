using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GwentClone.UI
{
    public class UI_ToolTipButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {


        public virtual void OnPointerEnter(PointerEventData eventData)
        {

        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            GlobalActions.OnStopHoveredUIButton?.Invoke();
        }


    }

}

