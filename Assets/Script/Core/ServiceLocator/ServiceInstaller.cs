using UnityEngine;
namespace Game.Core
{
    public abstract class ServiceInstaller : MonoBehaviour
    {
        protected virtual void Awake()
        {
            InstallServices();
        }
        protected abstract void InstallServices();
        protected void RegisterService<T>(T service) where T : IService
        {
            ServiceLocator.Instance.Register(service);
        }
    }
}