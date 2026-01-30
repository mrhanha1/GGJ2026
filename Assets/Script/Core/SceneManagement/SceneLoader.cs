using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    public class SceneLoader : MonoBehaviour, IService
    {
        public event Action<string> OnSceneLoadStarted;
        public event Action<string, float> OnSceneLoadProgress; // sceneName, progress (0-1)
        public event Action<string> OnSceneLoadCompleted;

        private bool _isLoading = false;

        public void Initialize()
        {
            Debug.Log("[SceneLoader] Service initialized.");
        }

        public void LoadScene(string sceneName, Action onComplete = null)
        {
            if (_isLoading)
            {
                Debug.LogWarning($"[SceneLoader] Already loading a scene. Ignoring request to load {sceneName}.");
                return;
            }

            StartCoroutine(LoadSceneAsync(sceneName, onComplete));
        }

        private IEnumerator LoadSceneAsync(string sceneName, Action onComplete)
        {
            _isLoading = true;
            OnSceneLoadStarted?.Invoke(sceneName);
            Debug.Log($"[SceneLoader] Started loading scene: {sceneName}");

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                OnSceneLoadProgress?.Invoke(sceneName, progress);
                yield return null;
            }

            OnSceneLoadProgress?.Invoke(sceneName, 1f);

            yield return new WaitForSeconds(0.5f);

            operation.allowSceneActivation = true;

            while (!operation.isDone)
            {
                yield return null;
            }

            _isLoading = false;
            OnSceneLoadCompleted?.Invoke(sceneName);
            Debug.Log($"[SceneLoader] Completed loading scene: {sceneName}");

            onComplete?.Invoke();
        }
        public bool IsLoading => _isLoading;
    }
}