using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class ButtonHoverTooltip : UI_InstantiateToMainCanvas
    {
        private GameObject currentTooltip = null;
        private UI_HoverTooltip tooltipComp = null;

        private void OnEnable()
        {
            GlobalActions.OnHoveredUIButton += DisplayTooltipOnHover;
            GlobalActions.OnStopHoveredUIButton += HideTooltip;
        }

        private void OnDisable()
        {
            GlobalActions.OnHoveredUIButton -= DisplayTooltipOnHover;
            GlobalActions.OnStopHoveredUIButton -= HideTooltip;
        }

        private void DisplayTooltipOnHover(Card card = null, Leader leader = null)
        {
            if(currentTooltip == null)
            {
                currentTooltip = Instantiate(m_instantiatePrefab, _mainCanvas.transform);
                tooltipComp = currentTooltip.GetComponent<UI_HoverTooltip>();
            }

            if (tooltipComp == null)
            {
                Debug.LogWarning("Add tooltip component to your tooltip object.");
                return;
            }

            if (card != null && leader != null)
            {
                Debug.LogWarning("Should only display one at a time.");
                return;
            }

            if(card == null) tooltipComp.PassInfoToTooltip(leader);
            else tooltipComp.PassInfoToTooltip(card);


            currentTooltip.SetActive(true);
            

        }

        private void HideTooltip()
        {
            if (currentTooltip == null) return;
            currentTooltip.SetActive(false);
        }
    }

}

