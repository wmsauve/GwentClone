using UnityEngine;

public class SimpleAnimations : MonoBehaviour
{
    protected bool _beginAnim = false;
    protected float delta;
    protected float counter = 0f;
    public bool BeginAnimation { get { return _beginAnim; } set { _beginAnim = value; } }

    [Header("Animation Parameters")]
    [SerializeField] protected float m_animationSpeed = 1f;
    [SerializeField] protected float m_animationDuration = 1f;
    [SerializeField] protected EnumAnimDirection m_animationDirection = EnumAnimDirection.Nothing;

    protected virtual void Update()
    {
        if (!_beginAnim) return;
        delta = Time.deltaTime;
        counter += delta * m_animationSpeed;
    }

    protected virtual float GetAnimationDirection(EnumAnimDirection dir)
    {
        return 0f;
    }

}
