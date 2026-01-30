using Game.Core;
using Game.Services.Camera;
using UnityEngine;

public class GameplayInstaller : ServiceInstaller
{
    [SerializeField] private CameraService cameraService;

    protected override void InstallServices()
    {
        if (cameraService != null && !ServiceLocator.Instance.IsRegistered<ICameraService>())
        {
            RegisterService<ICameraService>(cameraService);
        }
    }
}