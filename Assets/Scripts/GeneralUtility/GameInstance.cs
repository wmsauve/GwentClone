using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class GameInstance : MonoBehaviour
    {
        private static GameInstance _instance;
        public static GameInstance Instance { get { return _instance; } }

        private static FloatingMessage _floatingMessage;
        public static FloatingMessage FloatingMessage { get { return _floatingMessage; } }

        private void Awake()
        {
            Debug.Log("Main Initialization: Initializing GameInstance.");
            _floatingMessage = GetComponent<FloatingMessage>();
            if(_floatingMessage == null)
            {
                Debug.LogWarning("Your GameInstance is missing a FloatingMessage component.");
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

}
