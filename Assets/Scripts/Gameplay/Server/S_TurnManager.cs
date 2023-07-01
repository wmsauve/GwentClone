using UnityEngine;
using Unity.Netcode;
using TMPro;

public class S_TurnManager : NetworkBehaviour
{
    private int _turns = 0;
    private int _matchCounter = 0;
    private bool _turnPassed = false;
        
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
                SpawnFloatingMessageClientRpc();
                GlobalActions.OnGameStart?.Invoke();
            }
            else EndGameTimer();
        }
    }

    [Header("Timings related")]
    public float _coinFlipDuration = 5f;
    public float _mulliganPhase = 45f;
    public float _turnDuration = 30f;
    public float _afterMatchDurations = 10f;
    private float _currentTimer;

    [Header("UI Related")]
    public TextMeshProUGUI _timerObj = null;

    private void Update()
    {
        if (IsClient)
        {
            if (_timerObj == null) return;
            _timerObj.text = Mathf.Floor(_turnCount.Value).ToString();
        }

        if (!_gameStart.Value) return;

        if (IsServer)
        {
            IncrementTurnCount();
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
                    EndRegularTurn(true);
                    break;
                case EnumGameplayPhases.MatchOver:
                    EndBetweenMatchesPeriod();
                    break;
                case EnumGameplayPhases.GameOver:
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
        _matchCounter++;
        GlobalActions.OnPhaseChange?.Invoke(_currentPhase.Value);
    }

    public void EndRegularTurn(bool turnPassed)
    {
        _turnCount.Value = 0f;
       
        if (_turnPassed)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, $"Match {_matchCounter} ended.");
            _matchCounter++;
            _currentPhase.Value = EnumGameplayPhases.MatchOver;
            _currentTimer = _afterMatchDurations;
        }
        else if (turnPassed && !_turnPassed)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, $"Turn {_turns} ended.");
            _turnPassed = turnPassed;
        }

        _turns++;
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

    private void EndBetweenMatchesPeriod()
    {
        _currentTimer = _turnDuration;
        _turnPassed = false;
        _currentPhase.Value = EnumGameplayPhases.Regular;
        _turnCount.Value = 0f;
        GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, $"Begin the next match: {_matchCounter}");
        GlobalActions.OnPhaseChange?.Invoke(_currentPhase.Value);
    }

    private void EndGameTimer()
    {
        _timerObj.text = "0";
        _currentPhase.Value = EnumGameplayPhases.GameOver;
        GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.ServerProgression, "Turns Ended. Game Over.");
        GlobalActions.OnPhaseChange?.Invoke(_currentPhase.Value);
    }

    #region FloatingMessage
    [ClientRpc]
    private void SpawnFloatingMessageClientRpc()
    {
        GlobalActions.OnDisplayFeedbackInUI?.Invoke("Dooty doot goes first yo.");
    }
    #endregion FloatingMessage
}
