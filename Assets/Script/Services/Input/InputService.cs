using UnityEngine;

namespace Game.Services.Input
{
    /// <summary>
    /// Centralized input management service.
    /// Holds a single GameInputActions instance and exposes action maps.
    /// </summary>
    public class InputService : IInputService
    {
        private GameInputActions _inputActions;

        public GameInputActions.PlayerActions Player => _inputActions.Player;
        public GameInputActions.UIActions UI => _inputActions.UI;
        public GameInputActions.GameStateActions GameState => _inputActions.GameState;
        public GameInputActions.PuzzleActions Puzzle => _inputActions.Puzzle;

        public void Initialize()
        {
            _inputActions = new GameInputActions();
            EnableAllInputs();
            Debug.Log("[InputService] Initialized - All action maps enabled.");
        }

        public void EnableAllInputs()
        {
            _inputActions.Enable();
        }

        public void DisableAllInputs()
        {
            _inputActions.Disable();
        }
    }
}