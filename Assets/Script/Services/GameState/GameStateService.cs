// Assets/_Project/Scripts/Services/GameState/GameStateService.cs

using System;
using UnityEngine;

namespace Game.Services.GameState
{
    /// <summary>
    /// Game state service implementation.
    /// Manages game state transitions and persistence via PlayerPrefs.
    /// </summary>
    public class GameStateService : MonoBehaviour, IGameStateService
    {
        private const string LAST_STATE_KEY = "GameState_LastState";

        private GameState _currentState = GameState.Menu;

        // Events
        public event Action<GameState> OnStateChanged;
        public event Action OnWin;
        public event Action OnLose;
        public event Action OnRestart;

        public GameState CurrentState => _currentState;

        public void Initialize()
        {
            // Load last state from PlayerPrefs (optional)
            if (PlayerPrefs.HasKey(LAST_STATE_KEY))
            {
                int lastState = PlayerPrefs.GetInt(LAST_STATE_KEY);
                _currentState = (GameState)lastState;
                Debug.Log($"[GameStateService] Loaded last state: {_currentState}");
            }

            Debug.Log("[GameStateService] Initialized.");
        }

        // ===== STATE MANAGEMENT =====
        public void SetState(GameState newState)
        {
            if (_currentState == newState)
            {
                Debug.LogWarning($"[GameStateService] Already in state: {newState}");
                return;
            }

            GameState oldState = _currentState;
            _currentState = newState;

            PlayerPrefs.SetInt(LAST_STATE_KEY, (int)_currentState);
            PlayerPrefs.Save();

            Debug.Log($"[GameStateService] State changed: {oldState} → {_currentState}");
            OnStateChanged?.Invoke(_currentState);
        }
        private bool IsTerminalState(GameState state)
        {
            return state == GameState.Win || state == GameState.Lose;
        }

        // ===== WIN/LOSE =====
        public void TriggerWin()
        {
            if (IsTerminalState(_currentState))
            {
                Debug.LogWarning($"[GameStateService] Cannot trigger Win from state: {_currentState}");
                return;
            }

            _currentState = GameState.Win;
            PlayerPrefs.SetInt(LAST_STATE_KEY, (int)_currentState);
            PlayerPrefs.Save();

            Debug.Log("[GameStateService] 🎉 WIN!");
            OnStateChanged?.Invoke(_currentState);
            OnWin?.Invoke();
        }

        public void TriggerLose()
        {
            if (IsTerminalState(_currentState))
            {
                Debug.LogWarning($"[GameStateService] Cannot change state from terminal state: {_currentState}. Use Restart() to reset.");
                return;
            }
            SetState(GameState.Lose);
            Debug.Log("[GameStateService] 💀 LOSE!");
            OnLose?.Invoke();
        }

        public void Restart()
        {
            _currentState = GameState.Playing;
            SetState(GameState.Playing);
            Debug.Log("[GameStateService] 🔄 RESTART!");
            OnRestart?.Invoke();
        }
    }
}