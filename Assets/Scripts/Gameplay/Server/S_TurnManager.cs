using UnityEngine;
using Unity.Netcode;
using TMPro;

public class S_TurnManager : NetworkBehaviour
{
    private int _turns = 0;
        
    private NetworkVariable<float> _turnCount = new NetworkVariable<float>(0f);
    private NetworkVariable<EnumGameplayPhases> _currentPhase = new NetworkVariable<EnumGameplayPhases>(EnumGameplayPhases.CoinFlip);
    private NetworkVariable<bool> _gameStart = new NetworkVariable<bool>(false);
    public bool GameStart
    {
        get { return _gameStart.Value; }
        set
        {
            _gameStart.Value = value;
            if (value)
            {
                _currentTimer = _coinFlipDuration;
                GlobalActions.OnGameStart?.Invoke();
            }
            else EndGameTimer();
        }
    }

    [Header("Timings related")]
    public float _coinFlipDuration = 5f;
    public float _mulliganPhase = 45f;
    public float _turnDuration = 30f;
    private float _currentTimer;

    [Header("UI Related")]
    public TextMeshProUGUI _timerObj = null;

    private void Update()
    {
        if (!_gameStart.Value) return;

        if (IsServer)
        {
            IncrementTurnCount();
        }

        if (IsClient)
        {
            if (_timerObj == null) return;
            _timerObj.text = Mathf.Floor(_turnCount.Value).ToString();
        }
    }

    private void IncrementTurnCount()
    {
        _turnCount.Value += Time.deltaTime;
        if(_turnCount.Value >= _currentTimer)
        {
            switch (_currentPhase.Value)
            {
                case EnumGameplayPhases.CoinFlip:
                    EndCoinFlip();
                    break;
                case EnumGameplayPhases.Mulligan:
                    EndMulliganPhase();
                    break;
                case EnumGameplayPhases.Regular:
                    EndRegularTurn();
                    break;
                default:
                    GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Error with ending phases.");
                    break;
            }
        }
    }

    public void EndMulliganPhase()
    {
        _currentPhase.Value = EnumGameplayPhases.Regular;
        _currentTimer = _turnDuration;
        _turnCount.Value = 0f;
        GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, "Mulligan phase ended.");
        GlobalActions.OnPhaseChange?.Invoke(_currentPhase.Value);
    }

    public void EndRegularTurn()
    {
        _turnCount.Value = 0f;
        _turns++;
        GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, "Turn ended.");
        GlobalActions.OnPhaseChange?.Invoke(_currentPhase.Value);
    }

    private void EndCoinFlip()
    {
        _currentPhase.Value = EnumGameplayPhases.Mulligan;
        _currentTimer = _mulliganPhase;
        _turnCount.Value = 0f;
        GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, "Coinflip phase ended.");
        GlobalActions.OnPhaseChange?.Invoke(_currentPhase.Value);
    }

    private void EndGameTimer()
    {
        _timerObj.text = "0";
        _currentPhase.Value = EnumGameplayPhases.GameOver;
        GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, "Turns Ended. Game Over.");
    }
}
