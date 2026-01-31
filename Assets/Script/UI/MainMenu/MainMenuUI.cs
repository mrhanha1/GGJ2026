// Assets/_Project/Scripts/UI/MainMenu/MainMenuUI.cs

using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.UI
{
    /// <summary>
    /// Main menu UI controller - manages 3 panels: Start, LevelSelect, Settings
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Panels (Same Level)")]
        [SerializeField] private GameObject startPanel;        // Main menu with Play/Settings/Exit buttons
        [SerializeField] private LevelSelectPanel levelSelectPanel;
        [SerializeField] private SettingsPanel settingsPanel;

        [Header("Start Panel Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;

        private SceneLoader _sceneLoader;

        private void Start()
        {
            _sceneLoader = ServiceLocator.Instance.Get<SceneLoader>();

            // Setup button listeners
            playButton.onClick.AddListener(OnPlayClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            exitButton.onClick.AddListener(OnExitClicked);

            // Setup panel back buttons (if they exist)
            if (levelSelectPanel != null)
                levelSelectPanel.OnBackRequested += ShowStartPanel;

            if (settingsPanel != null)
                settingsPanel.OnBackRequested += ShowStartPanel;

            if (PlayerPrefs.GetInt("ShowLevelSelect", 0) == 1)
            {
                PlayerPrefs.DeleteKey("ShowLevelSelect");
                SetActivePanel(levelSelectPanel?.gameObject);
            }
            else
            {
                ShowStartPanel();
            }
        }

        /// <summary>
        /// Show Start Panel (hide others)
        /// </summary>
        private void ShowStartPanel()
        {
            SetActivePanel(startPanel);
        }

        /// <summary>
        /// Show Level Select Panel (hide others)
        /// </summary>
        private void OnPlayClicked()
        {
            SetActivePanel(levelSelectPanel?.gameObject);
        }

        /// <summary>
        /// Show Settings Panel (hide others)
        /// </summary>
        private void OnSettingsClicked()
        {
            SetActivePanel(settingsPanel?.gameObject);
        }

        /// <summary>
        /// Exit game
        /// </summary>
        private void OnExitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// Set active panel and hide others
        /// </summary>
        private void SetActivePanel(GameObject activePanel)
        {
            if (startPanel != null)
                startPanel.SetActive(startPanel == activePanel);

            if (levelSelectPanel != null)
                levelSelectPanel.gameObject.SetActive(levelSelectPanel.gameObject == activePanel);

            if (settingsPanel != null)
                settingsPanel.gameObject.SetActive(settingsPanel.gameObject == activePanel);
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
            exitButton.onClick.RemoveListener(OnExitClicked);

            if (levelSelectPanel != null)
                levelSelectPanel.OnBackRequested -= ShowStartPanel;

            if (settingsPanel != null)
                settingsPanel.OnBackRequested -= ShowStartPanel;
        }
    }
}