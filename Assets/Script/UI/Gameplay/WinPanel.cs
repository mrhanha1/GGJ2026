// Assets/_Project/Scripts/UI/Gameplay/WinPanel.cs

using UnityEngine;
using UnityEngine.UI;
using Game.Core;
using PuzzleGame.Gameplay;

namespace Game.UI
{
    /// <summary>
    /// Win panel UI.
    /// </summary>
    public class WinPanel : UIPanel
    {
        [Header("Buttons")]
        [SerializeField] private Button nextButton; // NEW
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private SceneLoader _sceneLoader;
        private PuzzleManager _puzzleManager; // NEW

        protected override void Awake()
        {
            base.Awake();

            _sceneLoader = ServiceLocator.Instance.Get<SceneLoader>();
            _puzzleManager = FindObjectOfType<PuzzleManager>(); // NEW

            // NEW: Next button
            if (nextButton != null)
                nextButton.onClick.AddListener(OnNextClicked);

            restartButton.onClick.AddListener(OnRestartClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        // NEW: Next level logic
        private void OnNextClicked()
        {
            if (_puzzleManager != null)
            {
                _puzzleManager.NextLevel();
                Hide();
            }
        }

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
            if (nextButton != null)
                nextButton.onClick.RemoveListener(OnNextClicked);

            restartButton.onClick.RemoveListener(OnRestartClicked);
            mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        }
    }
}