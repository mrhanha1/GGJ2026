using Game.Core;
using Game.Services.GameState;
using Game.Services.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateTest : MonoBehaviour
{
    private IGameStateService _gameState;
    private IInputService _inputService;

    void Start()
    {
        _gameState = ServiceLocator.Instance.Get<IGameStateService>();
        _inputService = ServiceLocator.Instance.Get<IInputService>();

        // Subscribe to GameState actions
        _inputService.GameState.TriggerWin.performed += OnTriggerWin;
        _inputService.GameState.TriggerLose.performed += OnTriggerLose;
        _inputService.GameState.Restart.performed += OnRestartInput;

        // Subscribe to GameState events
        _gameState.OnStateChanged += OnStateChanged;
        _gameState.OnWin += OnWin;
        _gameState.OnLose += OnLose;
        _gameState.OnRestart += OnRestart;

        _gameState.SetState(GameState.Playing);
    }

    // Input callbacks
    private void OnTriggerWin(InputAction.CallbackContext context)
    {
        _gameState.TriggerWin();
    }

    private void OnTriggerLose(InputAction.CallbackContext context)
    {
        _gameState.TriggerLose();
    }

    private void OnRestartInput(InputAction.CallbackContext context)
    {
        _gameState.Restart();
    }

    // GameState event callbacks
    private void OnStateChanged(GameState newState)
    {
        Debug.Log($"[TEST] State Changed Event: {newState}");
    }

    private void OnWin()
    {
        Debug.Log("[TEST] Win Event Triggered!");
        _inputService.Player.Disable();
    }

    private void OnLose()
    {
        Debug.Log("[TEST] Lose Event Triggered!");
        _inputService.Player.Disable();
    }

    private void OnRestart()
    {
        Debug.Log("[TEST] Restart Event Triggered!");
        _inputService.Player.Enable();
    }

    void OnDestroy()
    {
        if (_inputService != null)
        {
            _inputService.GameState.TriggerWin.performed -= OnTriggerWin;
            _inputService.GameState.TriggerLose.performed -= OnTriggerLose;
            _inputService.GameState.Restart.performed -= OnRestartInput;
        }

        if (_gameState != null)
        {
            _gameState.OnStateChanged -= OnStateChanged;
            _gameState.OnWin -= OnWin;
            _gameState.OnLose -= OnLose;
            _gameState.OnRestart -= OnRestart;
        }
    }
}