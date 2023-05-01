using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [Tooltip("The Instance of Monobehavior T will be this one. Make sure you only check true for one of each type.")]
    [SerializeField] private bool m_masterInstance = false;

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(T).Name);
                    _instance = singleton.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    

    protected virtual void Awake()
    {

        Debug.LogWarning("Yoyoyoyoyor4r4r " + gameObject.name);

        if (_instance == null && m_masterInstance)
        {
            _instance = this as T;
            Debug.LogWarning("Yoyoyoyoyo " + gameObject.name);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Yoyoyoyoyo222 " + gameObject.name);
            Destroy(this);
        }
    }
}