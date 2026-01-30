// Assets/_Project/Scripts/UI/MainMenu/MainMenuUI.cs

using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.UI
{
    /// <summary>
    /// Main menu UI controller.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;

        [Header("Panels")]
        [SerializeField] private SettingsPanel settingsPanel;

        private SceneLoader _sceneLoader;

        private void Start()
        {
            _sceneLoader = ServiceLocator.Instance.Get<SceneLoader>();

            playButton.onClick.AddListener(OnPlayClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            exitButton.onClick.AddListener(OnExitClicked);

            if (settingsPanel != null)
                settingsPanel.gameObject.SetActive(false);
        }

        private void OnPlayClicked()
        {
            _sceneLoader.LoadScene(SceneNames.Gameplay);
        }

        private void OnSettingsClicked()
        {
            if (settingsPanel != null)
                settingsPanel.Show();
        }

        private void OnExitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
            exitButton.onClick.RemoveListener(OnExitClicked);
        }
    }
}