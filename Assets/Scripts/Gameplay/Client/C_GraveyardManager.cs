using UnityEngine;
using UnityEngine.UI;

public class C_GraveyardManager : MonoBehaviour
{
    [SerializeField] private Button m_toggleGYButton = null;

    private void Start()
    {
        var init = m_toggleGYButton.gameObject.GetComponent<UI_OnButtonHover>();
        if (m_toggleGYButton == null || init == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Need PassTurn Button.");
            return;
        }
    }

    private void OnEnable()
    {
        m_toggleGYButton.onClick.AddListener(ToggleGraveyardUI);
    }

    private void OnDisable()
    {
        m_toggleGYButton.onClick.RemoveListener(ToggleGraveyardUI);
    }

    private void ToggleGraveyardUI()
    {

    }
}
