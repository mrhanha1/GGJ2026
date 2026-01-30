using Game.Core;
using Game.Services.Audio;
using Game.Services.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioServiceTest : MonoBehaviour
{
    [SerializeField] private AudioData audioData;
    private IAudioService _audioService;
    private IInputService _inputService;

    void Start()
    {
        _audioService = ServiceLocator.Instance.Get<IAudioService>();
        _inputService = ServiceLocator.Instance.Get<IInputService>();

        // Subscribe to UI.Click action
        _inputService.UI.Click.performed += OnTestClick;

        if (audioData.mainMenuMusic != null)
        {
            _audioService.PlayMusic(audioData.mainMenuMusic);
        }
    }

    void Update()
    {
        // Volume test với keyboard
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("Testing Master Volume = 0.5");
            _audioService.SetMasterVolume(0.5f);
        }

        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("Testing Music Volume = 0.7");
            _audioService.SetMusicVolume(0.7f);
        }

        if (keyboard.digit3Key.wasPressedThisFrame)
        {
            Debug.Log("Testing SFX Volume = 0.3");
            _audioService.SetSFXVolume(0.3f);
        }
    }

    private void OnTestClick(InputAction.CallbackContext context)
    {
        if (audioData.buttonClickSFX != null)
        {
            _audioService.PlaySFX(audioData.buttonClickSFX);
        }
    }

    void OnDestroy()
    {
        if (_inputService != null)
        {
            _inputService.UI.Click.performed -= OnTestClick;
        }
    }
}