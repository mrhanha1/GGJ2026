using Game.Core;
using Game.Services.Audio;
using Game.Services.GameState;
using UnityEngine;

public class MainMenuInstaller : ServiceInstaller
{
    [SerializeField] private AudioData audioData;

    protected override void InstallServices()
    {
        // Audio Service
        if (!ServiceLocator.Instance.IsRegistered<IAudioService>())
        {
            GameObject audioObj = new GameObject("AudioService");
            audioObj.transform.SetParent(ServiceLocator.Instance.transform);
            var audioService = audioObj.AddComponent<AudioService>();
            RegisterService<IAudioService>(audioService);
        }

        // GameState Service
        if (!ServiceLocator.Instance.IsRegistered<IGameStateService>())
        {
            GameObject stateObj = new GameObject("GameStateService");
            stateObj.transform.SetParent(ServiceLocator.Instance.transform);
            var stateService = stateObj.AddComponent<GameStateService>();
            RegisterService<IGameStateService>(stateService);
        }
    }
}