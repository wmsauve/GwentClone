using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackendFunctionality;

namespace GwentClone
{
    public struct GwentUser
    {
        public string username;
    }

    public class GameInstance : MonoBehaviour
    {
        private static GameInstance _instance;
        public static GameInstance Instance { get { return _instance; } }

        private static GwentUser _user;
        public static GwentUser User { get { return _user; } set { _user = value; } }

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

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

}
