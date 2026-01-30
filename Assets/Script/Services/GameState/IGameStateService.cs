// Assets/_Project/Scripts/Services/GameState/IGameStateService.cs

using System;

namespace Game.Services.GameState
{
    /// <summary>
    /// Game states.
    /// </summary>
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        Win,
        Lose
    }

    /// <summary>
    /// Interface for game state management service.
    /// Handles state transitions, win/lose logic, and state persistence.
    /// </summary>
    public interface IGameStateService : Game.Core.IService
    {
        // State
        GameState CurrentState { get; }
        void SetState(GameState newState);

        // Win/Lose
        void TriggerWin();
        void TriggerLose();
        void Restart();

        // Events (C# events)
        event Action<GameState> OnStateChanged;
        event Action OnWin;
        event Action OnLose;
        event Action OnRestart;
    }
}