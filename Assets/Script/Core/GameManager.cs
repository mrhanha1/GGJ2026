using UnityEngine;

namespace Game.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool autoLoadMainMenu = true;
        [SerializeField] private float delayBeforeLoad = 0.5f;

        private SceneLoader _sceneLoader;

        private void Awake()
        {
            Debug.Log("[GameManager] Initializing game...");

            if (ServiceLocator.Instance == null)
            {
                Debug.LogError("[GameManager] ServiceLocator not found! Make sure ServiceLocator prefab is in the Bootstrap scene.");
                return;
            }

            RegisterCoreServices();

            Application.focusChanged += OnApplicationFocusChanged;
            Application.quitting += OnApplicationQuitting;

            Debug.Log("[GameManager] Initialization complete.");
        }

        private void Start()
        {
            if (autoLoadMainMenu)
            {
                Invoke(nameof(LoadMainMenu), delayBeforeLoad);
            }
        }

        private void RegisterCoreServices()
        {
            Debug.Log("[GameManager] Registering core services...");

            GameObject sceneLoaderObj = new GameObject("SceneLoader");
            sceneLoaderObj.transform.SetParent(ServiceLocator.Instance.transform);
            _sceneLoader = sceneLoaderObj.AddComponent<SceneLoader>();
            ServiceLocator.Instance.Register(_sceneLoader);

            var inputService = new Game.Services.Input.InputService();
            ServiceLocator.Instance.Register<Game.Services.Input.IInputService>(inputService);
        }

        private void LoadMainMenu()
        {
            Debug.Log("[GameManager] Loading MainMenu scene...");
            _sceneLoader.LoadScene(SceneNames.MainMenu);
        }

        private void OnApplicationFocusChanged(bool hasFocus)
        {
            if (hasFocus)
            {
                Debug.Log("[GameManager] Application gained focus.");
            }
            else
            {
                Debug.Log("[GameManager] Application lost focus.");
            }
        }

        private void OnApplicationQuitting()
        {
            Debug.Log("[GameManager] Application is quitting. Cleaning up...");
        }

        private void OnDestroy()
        {
            Application.focusChanged -= OnApplicationFocusChanged;
            Application.quitting -= OnApplicationQuitting;
        }
    }
}