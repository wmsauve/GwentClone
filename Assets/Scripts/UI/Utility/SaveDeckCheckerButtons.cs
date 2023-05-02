using UnityEngine;
using UnityEngine.UI;

public class SaveDeckCheckerButtons : MonoBehaviour
{
    [SerializeField] private Button m_acceptButton = null;
    [SerializeField] private Button m_cancelButton = null;

    private MonoBehaviour triggeredComponent = null;

    public void InitializeTheChecker(MonoBehaviour whoTriggered)
    {
        if (m_acceptButton == null || m_cancelButton == null)
        {
            Debug.LogWarning("Check your prefab and set your buttons");
            return;
        }

        if (whoTriggered == null)
        {
            Debug.LogWarning("Why did you not pass the target here?");
            return;
        }

        if(!(whoTriggered is ISaveDependentComponent))
        {
            Debug.LogWarning("The monobehavior trying to use this needs to implement ISaveDependentComponent.");
            return;
        }

        triggeredComponent = whoTriggered;
    }

    private void OnEnable()
    {
        if (m_acceptButton == null || m_cancelButton == null) return;

        m_cancelButton.onClick.AddListener(CancelButtonFunctionality);
        m_acceptButton.onClick.AddListener(SkipSavingFunctionality);
    }

    private void OnDisable()
    {
        if (m_acceptButton == null || m_cancelButton == null) return;
        m_cancelButton.onClick.RemoveListener(CancelButtonFunctionality);
        m_acceptButton.onClick.RemoveListener(SkipSavingFunctionality);
    }

    private void OnDestroy()
    {
        if (m_acceptButton == null || m_cancelButton == null) return;
        m_cancelButton.onClick.RemoveListener(CancelButtonFunctionality);
        m_acceptButton.onClick.RemoveListener(SkipSavingFunctionality);
    }

    private void CancelButtonFunctionality()
    {
        Destroy(gameObject);
    }

    private void SkipSavingFunctionality()
    {
        (triggeredComponent as ISaveDependentComponent).OnResolveSaveCheck();
        Destroy(gameObject);
    }

}
