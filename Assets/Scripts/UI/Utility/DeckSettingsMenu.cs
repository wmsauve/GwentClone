using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GwentClone.UI
{
    public class DeckSettingsMenu : MonoBehaviour
    {
        [Header("Components Of Menu")]
        [SerializeField] private TextMeshProUGUI m_deckNameHeader = null;
        [SerializeField] private TextMeshProUGUI m_leaderNameHeader = null;
        [SerializeField] private TMP_Dropdown m_leaderMenu = null;

        private Deck _whichDeck;
        private List<TMP_Dropdown.OptionData> _leaderList = new List<TMP_Dropdown.OptionData>();

        public void InitializeOnPopup(Deck deck)
        {
            _whichDeck = deck;

            m_deckNameHeader.text = _whichDeck.DeckName;
            m_leaderNameHeader.text = _whichDeck.DeckLeader.id;

            List<Leader> leaders = GameInstance.Instance.CardRepo.GetLeadersByFaction(_whichDeck.DeckLeader.factionType);
            foreach(Leader leader in leaders)
            {
                var newOption = new TMP_Dropdown.OptionData();
                newOption.text = leader.id;
                _leaderList.Add(newOption);
            }

            m_leaderMenu.options = _leaderList;
        }

    }

}

