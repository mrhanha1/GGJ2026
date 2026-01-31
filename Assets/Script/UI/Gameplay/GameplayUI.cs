using UnityEngine;
using UnityEngine.UI;
using PuzzleGame.Gameplay;

namespace PuzzleGame.UI.Gameplay
{
    /// <summary>
    /// Main UI controller for Gameplay scene
    /// </summary>
    public class GameplayUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PuzzleManager puzzleManager;

        [Header("UI Elements")]
        [SerializeField] private Text levelText;
        [SerializeField] private Text modeText;
        [Header("Test Buttons (Remove in production)")]
        [SerializeField] private Button completeButton;
        private void Start()
        {
            UpdateUI();
            if (completeButton != null)
            {
                completeButton.onClick.AddListener(TestCompleteLevel);
            }
        }

        private void Update()
        {
            // Update UI if level changes
            UpdateUI();
        }

        /// <summary>
        /// Update level and mode display
        /// </summary>
        public void UpdateUI()
        {
            if (puzzleManager == null)
                return;

            // Update level text
            if (levelText != null)
            {
                int currentLevel = puzzleManager.CurrentLevelIndex + 1;
                int totalLevels = puzzleManager.TotalLevels;
                levelText.text = $"Level {currentLevel}/{totalLevels}";
            }

            // Update mode text
            if (modeText != null && puzzleManager.CurrentLevel != null)
            {
                //var board = FindObjectOfType<PuzzleGame.Gameplay.Board.PuzzleBoard>();
                var board = FindAnyObjectByType<PuzzleGame.Gameplay.Board.PuzzleBoard>();
                if (board != null && board.Settings != null)
                {
                    string fillMode = board.Settings.FillAllTiles ? "Fill All" : "Fill Targets";
                    string placeMode = board.Settings.RequireFullyInside ? "Inside Only" : "Partial OK";
                    modeText.text = $"{fillMode} | {placeMode}";
                }
            }
        }
        private void TestCompleteLevel()
        {
            if (puzzleManager != null)
            {
                puzzleManager.ForceCompleteLevel();
            }
        }
    }
}