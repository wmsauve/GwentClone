using UnityEngine;
using UnityEngine.EventSystems;

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

