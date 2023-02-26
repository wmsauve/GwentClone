using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GwentClone
{
    public class UI_Manager : MonoBehaviour
    {

        public static Action initializeAllUI;

        void Start()
        {

            Debug.Log("Main Initialization: Initialize all UI elements");
            initializeAllUI.Invoke();
        }


    }

}

