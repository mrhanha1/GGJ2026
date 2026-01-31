using UnityEngine;
using UnityEngine.UI;
using Game.Core;
using System;

namespace Game.UI
{
    /// <summary>
    /// Level selection panel UI
    /// </summary>
    public class LevelSelectPanel : MonoBehaviour
    {
        [Header("Level Buttons")]
        [SerializeField] private Button[] levelButtons; // 5 buttons for 5 levels

        [Header("Navigation")]
        [SerializeField] private Button backButton; // Back to main menu

        private SceneLoader _sceneLoader;
        private const string SELECTED_LEVEL_KEY = "SelectedLevel";

        // Event for back button
        public event Action OnBackRequested;

        private void Awake()
        {
            _sceneLoader = ServiceLocator.Instance.Get<SceneLoader>();

            // Setup level button listeners
            for (int i = 0; i < levelButtons.Length; i++)
            {
                int levelIndex = i; // Capture for closure
                if (levelButtons[i] != null)
                {
                    levelButtons[i].onClick.AddListener(() => OnLevelSelected(levelIndex));
                }
            }

            // Setup back button
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }
        }

        /// <summary>
        /// Called when a level button is clicked
        /// </summary>
        private void OnLevelSelected(int levelIndex)
        {
            // Save selected level index to PlayerPrefs
            PlayerPrefs.SetInt(SELECTED_LEVEL_KEY, levelIndex);
            PlayerPrefs.Save();

            Debug.Log($"[LevelSelectPanel] Selected level: {levelIndex + 1}");

            // Load gameplay scene
            _sceneLoader.LoadScene(SceneNames.Gameplay);
        }

        /// <summary>
        /// Called when back button is clicked
        /// </summary>
        private void OnBackClicked()
        {
            OnBackRequested?.Invoke();
        }

        /// <summary>
        /// Get the selected level index (called by PuzzleManager on gameplay start)
        /// </summary>
        public static int GetSelectedLevel()
        {
            return PlayerPrefs.GetInt(SELECTED_LEVEL_KEY, 0); // Default to level 0
        }

        private void OnDestroy()
        {
            // Cleanup listeners
            for (int i = 0; i < levelButtons.Length; i++)
            {
                if (levelButtons[i] != null)
                {
                    levelButtons[i].onClick.RemoveAllListeners();
                }
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(OnBackClicked);
            }
        }
    }
}