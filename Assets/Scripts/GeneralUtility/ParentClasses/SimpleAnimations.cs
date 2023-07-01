using UnityEngine;

public class SimpleAnimations : MonoBehaviour
{
    protected bool _beginAnim = false;
    protected float delta;
    protected float counter = 0f;
    public bool BeginAnimation { get { return _beginAnim; } set { _beginAnim = value; } }

    [Header("Animation Parameters")]
    [Tooltip("How fast is the animation doing stuff?")]
    [SerializeField] protected float m_animationSpeed = 1f;
    [Tooltip("How long should this be going on for?")]
    [SerializeField] protected float m_animationDuration = 1f;
    [SerializeField] protected EnumAnimDirection m_animationDirection = EnumAnimDirection.Nothing;

    protected virtual void Update()
    {
        if (!_beginAnim) return;
        delta = Time.deltaTime;
        counter += delta;
    }

    protected virtual float GetAnimationDirection(EnumAnimDirection dir)
    {
        return 0f;
    }

}
