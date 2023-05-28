using UnityEngine;
public class Anim_TransformUI : SimpleAnimations
{
    [Header("Object To Resize Related")]
    [SerializeField] private RectTransform _myTransform = null;
    [SerializeField] private bool m_useWorldTransformPos = false;
     
    private float _targetScale;
    private float _targetXPos;
    private float _targetYPos;

    private EnumMulliganPos _currentPos;
    public EnumMulliganPos CurrentPos { get { return _currentPos; } }

    private float _initalScale;
    private Vector2 _initialPos;

    private void Awake()
    {
        if(_myTransform == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You did not put your rect transform here.");
        }
    }

    protected override void Update()
    {
        if (!_beginAnim || _myTransform == null) return;

        base.Update();
        

        if(counter > 1.0f)
        {
            counter = 1.0f;
            _beginAnim = false;
        }

        var _scale = Mathf.Lerp(_initalScale, _targetScale, counter);
        var _pos = Vector2.Lerp(_initialPos, new Vector2(_targetXPos, _targetYPos), counter);

        _myTransform.localScale = new Vector3(_scale, _scale, _myTransform.localScale.z);
        if(!m_useWorldTransformPos) _myTransform.localPosition = _pos;
        else { _myTransform.position = _pos; }

    }

    public float BeginThisAnimation(AnimationMoveSpotParams _params, EnumMulliganPos newPos = EnumMulliganPos.center)
    {
        counter = 0;

        _targetScale = _params.CardScale;
        _targetYPos = _params.CardPosY;
        _targetXPos = _params.CardPosX;

        _beginAnim = true;
        _currentPos = newPos;

        _initalScale = _myTransform.localScale.x;
        if (!m_useWorldTransformPos) _initialPos = _myTransform.localPosition;
        else { _initialPos = _myTransform.position; }

        return m_animationSpeed;
    }

    public void InitializeMullgianCard(AnimationMoveSpotParams _params, EnumMulliganPos pos)
    {
        if (_myTransform == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "Missing Rect transform on button.");
            return;
        }

        _currentPos = pos;
        _myTransform.localScale = new Vector3(_params.CardScale, _params.CardScale, _params.CardScale);
        _myTransform.localPosition = new Vector2(_params.CardPosX, _params.CardPosY);
    }

    public void ResetThisObject()
    {
        _myTransform.localScale = new Vector3(_initalScale, _initalScale, _initalScale);

        if (!m_useWorldTransformPos) _myTransform.localPosition = _initialPos;
        else { _myTransform.position = _initialPos; }
        
        _beginAnim = false;
        counter = 0;
    }
}
