using TMPro;
using UnityEngine;

public class FloatingMessage : UI_InstantiateToMainCanvas
{

    private void OnEnable()
    {
        GlobalActions.OnDisplayFeedbackInUI += ShowMessage;
    }

    private void OnDisable()
    {
        GlobalActions.OnDisplayFeedbackInUI -= ShowMessage;
    }

    private void ShowMessage(string message)
    {
        if (m_instantiatePrefab == null || _mainCanvas == null) return;

        var newMessage = Instantiate(m_instantiatePrefab, _mainCanvas.transform);
        var textComp = newMessage.GetComponent<TextMeshProUGUI>();
        if (textComp == null) return;

        textComp.text = message;
        var animComp = newMessage.GetComponent<Anim_FloatingMessage>();
        if (animComp == null) return;
        animComp.BeginAnimation = true;
    }
}
