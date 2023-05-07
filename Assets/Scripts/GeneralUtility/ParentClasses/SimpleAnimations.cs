using UnityEngine;

public class SimpleAnimations : MonoBehaviour
{
    protected bool _beginAnim = false;
    protected float delta;
    protected float counter = 0f;
    public bool BeginAnimation { get { return _beginAnim; } set { _beginAnim = value; } }

    [Header("Animation Parameters")]
    public float m_animationSpeed = 1f;
    public float m_animationDuration = 1f;
    public EnumAnimDirection m_animationDirection = EnumAnimDirection.Upward;
    public EnumAnimEffect m_animationEffect = EnumAnimEffect.Nothing;

    protected virtual void Update()
    {
        if (!_beginAnim) return;
        delta = Time.deltaTime;
    }

    protected virtual float GetAnimationDirection(EnumAnimDirection dir)
    {
        switch (dir)
        {
            case EnumAnimDirection.Upward:
                return 1;
            case EnumAnimDirection.Downward:
                return -1;
            default:
                Debug.LogWarning("Check to see if this Enum is working correctly.");
                return 0;
        }
    }

}
