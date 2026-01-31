namespace Game.Services.Input
{
    /// <summary>
    /// Input service - provides centralized access to all input action maps
    /// </summary>
    public interface IInputService : Game.Core.IService
    {
        /// <summary>
        /// Get the Player action map (Movement, Combat, etc.)
        /// </summary>
        GameInputActions.PlayerActions Player { get; }

        /// <summary>
        /// Get the UI action map (Click, Navigate, etc.)
        /// </summary>
        GameInputActions.UIActions UI { get; }

        /// <summary>
        /// Get the GameState action map (Test controls: Win, Lose, Restart)
        /// </summary>
        GameInputActions.GameStateActions GameState { get; }

        GameInputActions.PuzzleActions Puzzle { get; }

        /// <summary>
        /// Enable all action maps
        /// </summary>
        void EnableAllInputs();

        /// <summary>
        /// Disable all action maps
        /// </summary>
        void DisableAllInputs();
    }
}