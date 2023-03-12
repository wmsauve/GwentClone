using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class UI_ScrollView : UI_InitializeFromManager
    {
        [Header("Scrollview Specific Related")]
        [SerializeField] protected Transform m_content = null;
        [SerializeField] protected GameObject m_buttonPrefab = null;

        protected virtual void Awake()
        {
            if(m_content == null)
            {
                Debug.LogWarning("You didn't add a content reference to this scrollview component.");
            }

            if(m_buttonPrefab == null)
            {
                Debug.LogWarning("You didn't add a prefab to instantiate to this scrollview component.");
            }
        }
    }

}
