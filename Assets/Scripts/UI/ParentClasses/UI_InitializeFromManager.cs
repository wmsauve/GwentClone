using UnityEngine;

public class UI_InitializeFromManager : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        GlobalActions.OnInitializeAllUI += InitializeThisUIComp;
    }

    protected virtual void OnDisable()
    {
        GlobalActions.OnInitializeAllUI -= InitializeThisUIComp;
    }

    public virtual void InitializeThisUIComp()
    {
    }
}


