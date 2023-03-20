using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GwentClone
{
    public class UI_InstantiateToMainCanvas : MonoBehaviour
    {
        [Header("Prefab Related")]
        [SerializeField] protected GameObject m_instantiatePrefab = null;
        protected GameObject _mainCanvas = null;

        protected virtual void Start()
        {
            if (m_instantiatePrefab == null)
            {
                Debug.LogWarning("You are not instantiating an object from the game instance.");
            }

            _mainCanvas = GameObject.FindGameObjectWithTag(GlobalConstantValues.TAG_MAINCANVAS);
            if (_mainCanvas == null)
            {
                Debug.LogWarning("Did you forget to add a MainUICanvas tag? Do you have a Canvas in your scene?");
            }

        }
    }

}
