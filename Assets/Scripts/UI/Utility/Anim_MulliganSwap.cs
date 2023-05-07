using UnityEngine;
public class Anim_MulliganSwap : SimpleAnimations
{

    [SerializeField] private RectTransform _myTransform = null;

    private float _targetScale;
    private float _targetXPos;
    private float _targetYPos;

    private EnumMulliganPos _currentPos;
    public EnumMulliganPos CurrentPos { get { return _currentPos; } }

    protected override void Update()
    {
        if (!_beginAnim || _myTransform == null) return;

        base.Update();
        counter += delta;

        if(counter > m_animationDuration)
        {
            counter = m_animationDuration;
            _beginAnim = false;
        }

        Debug.LogWarning(counter + " asd");

        var _scale = Mathf.Lerp(_myTransform.localScale.x, _targetScale, counter);
        var _pos = Vector2.Lerp(_myTransform.localPosition, new Vector2(_targetXPos, _targetYPos), counter);

        _myTransform.localScale = new Vector3(_scale, _scale, _myTransform.localScale.z);
        _myTransform.localPosition = _pos;

    }

    public void BeginThisAnimation(MulliganSpotParams _params, EnumMulliganPos newPos)
    {
        if (_beginAnim) return;

        counter = 0;

        _targetScale = _params.CardScale;
        _targetYPos = _params.CardPosY;
        _targetXPos = _params.CardPosX;

        _beginAnim = true;
        _currentPos = newPos;
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
