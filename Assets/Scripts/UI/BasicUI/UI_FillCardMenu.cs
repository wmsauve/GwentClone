using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GwentClone.UI
{
    public class UI_FillCardMenu : UI_ScrollView
    {


        [Header("Filter Related")]
        [SerializeField] private Button m_closeCombat = null;
        [SerializeField] private Button m_ranged = null;
        [SerializeField] private Button m_siege = null;

        private bool allowFilter = false;
        private EnumFactionType cacheFactionType;
        private List<UI_CardButton> cardButtons = new List<UI_CardButton>();
        private Button currentFilter = null;

        protected override void InitializeThisUIComp()
        {
            if (m_closeCombat == null || m_ranged == null || m_siege == null)
            {
                Debug.LogWarning("You don't have the filter buttons set.");
                return;
            }

            var allCards = GameInstance.CardRepo.AllCards;

            foreach (Card _card in allCards)
            {
                CreateCardSelectButton(_card);
            }

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_closeCombat == null || m_ranged == null || m_siege == null) return;


            m_closeCombat.onClick.AddListener(FilterCloseCombat);
            m_ranged.onClick.AddListener(FilterRanged);
            m_siege.onClick.AddListener(FilterSiege);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (m_closeCombat == null || m_ranged == null || m_siege == null) return;

            m_closeCombat.onClick.RemoveListener(FilterCloseCombat);
            m_ranged.onClick.RemoveListener(FilterRanged);
            m_siege.onClick.RemoveListener(FilterSiege);
        }

        public void ResetListBasedOnFaction(EnumFactionType faction)
        {
            cacheFactionType = faction;
            allowFilter = true;

            foreach (UI_CardButton _card in cardButtons)
            {
                var cardData = _card.CardData;
                var cardObj = _card.gameObject;

                if (cardData.factionType != faction && cardData.factionType != EnumFactionType.Neutral) cardObj.SetActive(false);
                else cardObj.SetActive(true);
            }
        }

        private void FilterCloseCombat()
        {
            if (!allowFilter) return;
            ToggleOffBasedOnPosition(EnumUnitPlacement.Frontline);
            ResetCurrentFilter(m_closeCombat);
        }
        private void FilterRanged()
        {
            if (!allowFilter) return;
            ToggleOffBasedOnPosition(EnumUnitPlacement.Ranged);
            ResetCurrentFilter(m_ranged);
        }
        private void FilterSiege()
        {
            if (!allowFilter) return;
            ToggleOffBasedOnPosition(EnumUnitPlacement.Siege);
            ResetCurrentFilter(m_siege);
        }

        private void CreateCardSelectButton(Card card)
        {
            if (cardButtons == null) return;

            var _newButton = Instantiate(m_buttonPrefab, m_content);
            var buttonComp = _newButton.GetComponent<UI_CardButton>();
            if (buttonComp == null)
            {
                Debug.LogWarning("Make sure your instantiated button has the right component on it.");
                return;
            }

            buttonComp.InitializeCardButton(card);
            cardButtons.Add(buttonComp);
        }

        private void ToggleOffBasedOnPosition(EnumUnitPlacement pos)
        {
            foreach (UI_CardButton _card in cardButtons)
            {
                var cardData = _card.CardData;
                var cardObj = _card.gameObject;

                if (cardData.unitPlacement == pos && (cardData.factionType == cacheFactionType || cardData.factionType == EnumFactionType.Neutral)) cardObj.SetActive(true);
                else cardObj.SetActive(false);

            }
        }

        private void ResetCurrentFilter(Button newButton)
        {
            var _newBtnImg = newButton.GetComponent<Image>();
            if (_newBtnImg == null) return;

            if (currentFilter == null)
            {
                currentFilter = newButton;
                _newBtnImg.color = Color.red;
                return;
            }

            var _img = currentFilter.GetComponent<Image>();
            if (_img == null) return;
            _img.color = Color.white;

            if (currentFilter == newButton)
            {
                ResetListBasedOnFaction(cacheFactionType);
                _newBtnImg.color = Color.white;
                return;
            }
            _newBtnImg.color = Color.red;
            currentFilter = newButton;
        }
    }

}

