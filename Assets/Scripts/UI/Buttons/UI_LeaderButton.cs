using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace GwentClone.UI
{
    public class UI_LeaderButton : UI_ToolTipButton
    {
        [SerializeField] private Image m_borderHighlight = null;
        [SerializeField] private Button m_buttonComp = null;
        [SerializeField] private TextMeshProUGUI m_leaderName = null;

        private Leader whichLeader = null;

        public void InitializeButton(Leader leader)
        {
            if(m_borderHighlight == null || m_buttonComp == null || m_leaderName == null)
            {
                Debug.LogWarning("Check that you put all your references in Leader button.");
                return;
            }

            m_leaderName.text = leader.id;

            switch (leader.factionType)
            {
                case EnumFactionType.Monsters:
                    m_borderHighlight.color = Color.red;
                    break;
                case EnumFactionType.Neutral:
                    m_borderHighlight.color = new Color(0f,0f,0f,0f);
                    break;
                case EnumFactionType.Nilfgaardian:
                    m_borderHighlight.color = Color.black;
                    break;
                case EnumFactionType.NorthernRealms:
                    m_borderHighlight.color = Color.blue;
                    break;
                case EnumFactionType.Scoiatael:
                    m_borderHighlight.color = Color.green;
                    break;
                case EnumFactionType.Skellige:
                    m_borderHighlight.color = Color.magenta;
                    break;

            }

            whichLeader = leader;
        }

        private void OnEnable()
        {
            if (m_buttonComp == null) return;
            m_buttonComp.onClick.AddListener(SelectedHero);
        }

        private void OnDisable()
        {
            if (m_buttonComp == null) return;
            m_buttonComp.onClick.RemoveListener(SelectedHero);
        }

        private void SelectedHero()
        {

        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (whichLeader == null) return;
            GlobalActions.OnHoveredUIButton?.Invoke(null, whichLeader);
        }
    }

}

