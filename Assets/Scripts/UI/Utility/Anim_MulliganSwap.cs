using UnityEngine;
public class Anim_MulliganSwap : SimpleAnimations
{

    [SerializeField] private RectTransform _myTransform = null;

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
        counter += delta * m_animationSpeed;

        if(counter > 1.0f)
        {
            counter = 1.0f;
            _beginAnim = false;
        }

        var _scale = Mathf.Lerp(_initalScale, _targetScale, counter);
        var _pos = Vector2.Lerp(_initialPos, new Vector2(_targetXPos, _targetYPos), counter);

        _myTransform.localScale = new Vector3(_scale, _scale, _myTransform.localScale.z);
        _myTransform.localPosition = _pos;

    }

    public float BeginThisAnimation(MulliganSpotParams _params, EnumMulliganPos newPos)
    {
        counter = 0;

        _targetScale = _params.CardScale;
        _targetYPos = _params.CardPosY;
        _targetXPos = _params.CardPosX;

        _beginAnim = true;
        _currentPos = newPos;

        _initalScale = _myTransform.localScale.x;
        _initialPos = _myTransform.localPosition;

        return m_animationSpeed;
    }

    public void InitializeMullgianCard(MulliganSpotParams _params, EnumMulliganPos pos)
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
}
