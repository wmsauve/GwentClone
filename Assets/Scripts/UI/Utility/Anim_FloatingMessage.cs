using UnityEngine;
using TMPro;

public class Anim_FloatingMessage : SimpleAnimations
{

    private RectTransform _myRect = null;
    private TextMeshProUGUI _myText = null;

    protected override void Update() 
    {
        base.Update();
        counter += delta;
        if(counter >= m_animationDuration)
        {
            Destroy(gameObject);
        }

        var _animDir = GetAnimationDirection(m_animationDirection);
        var _shiftAmt = delta * m_animationSpeed * _animDir;

        _myRect.position = new Vector2(_myRect.position.x, _myRect.position.y + _shiftAmt);

        if(m_animationEffect == EnumAnimEffect.Fade && _myText != null)
        {
            var alpha = 1 - (counter / (m_animationDuration + 0.0000001f));
            _myText.color = new Color(_myText.color.r, _myText.color.g, _myText.color.b, alpha);
        }
    }


    private void Awake()
    {
        _myRect = GetComponent<RectTransform>();
        if (_myRect == null)
        {
            Debug.LogWarning("This floating message should have a RectTransform.");
            return;
        }

        _myText = GetComponent<TextMeshProUGUI>();
        if (_myRect == null)
        {
            Debug.LogWarning("This floating message should have a TextMeshProUGUI.");
            return;
        }
    }

}

