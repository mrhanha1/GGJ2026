// Assets/_Project/Scripts/UI/Gameplay/LosePanel.cs

using UnityEngine;
using UnityEngine.UI;
using Game.Core;
using PuzzleGame.Gameplay;

namespace Game.UI
{
    /// <summary>
    /// Lose panel UI.
    /// </summary>
    public class LosePanel : UIPanel
    {
        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private SceneLoader _sceneLoader;
        private PuzzleManager _puzzleManager; // NEW

        protected override void Awake()
        {
            base.Awake();

            _sceneLoader = ServiceLocator.Instance.Get<SceneLoader>();
            _puzzleManager = FindAnyObjectByType<PuzzleManager>(); // NEW

            restartButton.onClick.AddListener(OnRestartClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        // UPDATED: Restart current level logic
        private void OnRestartClicked()
        {
            if (_puzzleManager != null)
            {
                _puzzleManager.RestartLevel();
                Hide();
            }
            else
            {
                // Fallback: reload scene
                _sceneLoader.LoadScene(SceneNames.Gameplay);
            }
        }

        private void OnMainMenuClicked()
        {
            _sceneLoader.LoadScene(SceneNames.MainMenu);
        }

        private void OnDestroy()
        {
            restartButton.onClick.RemoveListener(OnRestartClicked);
            mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        }
    }
}