using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackendFunctionality;

namespace GwentClone
{
    [System.Serializable]
    public struct GwentUser
    {
        public string username;
        public DeckInfo decks;

        [System.Serializable]
        public struct DeckInfo
        {
            public string _id;
            public string user;
            public Deck[] decks;
            public int __v;

            [System.Serializable]
            public struct Deck
            {
                public string name;
                public bool isCurrent;
                public Card[] cards;
                public string _id;
            }

            [System.Serializable]
            public struct Card
            {
                public string name;
                public string _id;
            }
        }
    }

    public class GameInstance : MonoBehaviour
    {
        private static GameInstance _instance;
        public static GameInstance Instance { get { return _instance; } }

        private static GwentUser _user;
        public static GwentUser User { get { return _user; } set { _user = value; } }

        private static CardRepository _cardRepo;
        public static CardRepository CardRepo { get { return _cardRepo; } }

        private void Awake()
        {
            Debug.Log("Main Initialization: Initializing GameInstance.");
            if(GetComponent<FloatingMessage>() == null)
            {
                Debug.LogWarning("Your GameInstance is missing a FloatingMessage component.");
            }

            if(GetComponent<ButtonHoverTooltip>() == null)
            {
                Debug.LogWarning("Your GameInstance is missing a ButtonHoverTooltip component.");
            }

            if(GetComponent<FunctionLibrary>() == null)
            {
                Debug.LogWarning("Your GameInstance is missing the function library.");
            }

            _cardRepo = GetComponent<CardRepository>();
            if(_cardRepo == null)
            {
                Debug.LogWarning("Your GameInstance is missing the card repository.");
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

}
