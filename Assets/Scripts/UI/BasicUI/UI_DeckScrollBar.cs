using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class UI_DeckScrollBar : UI_ScrollView
    {

        [Header("Saving Deck List Related")]
        [SerializeField] private GameObject m_deckChangeCheckerPrefab = null;

        protected override void Awake()
        {
            base.Awake();

            if (m_deckChangeCheckerPrefab == null)
            {
                Debug.LogWarning("You didn't add a prefab for checking if you are sure about changing decks without saving.");
            }
        }

        public void AddDeck(Deck _newDeck)
        {
            var _newBtn = Instantiate(m_buttonPrefab, m_content);
            var _btnComp = _newBtn.GetComponent<UI_DeckButton>();
            if (_btnComp == null)
            {
                Debug.LogWarning("Your button doesn't have the main functionality component.");
                return;
            }
            _btnComp.InitializeDeckButton(_newDeck, this);
        }

        public void TriggerDeckNotSavedYetWarning()
        {
            Instantiate(m_deckChangeCheckerPrefab, transform);
        }
    }

}
