using UnityEngine;
using PuzzleGame.Gameplay.Board;
using PuzzleGame.Gameplay.Level;

namespace PuzzleGame.Gameplay.Level
{
    /// <summary>
    /// Manages level loading and progression
    /// </summary>
    public class PuzzleManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PuzzleBoard board;

        [Header("Levels")]
        [SerializeField] private LevelConfig[] levels; // 7 levels
        [SerializeField] private int currentLevelIndex = 0;

        private bool isLevelComplete = false;

        private void Start()
        {
            if (levels == null || levels.Length == 0)
            {
                Debug.LogError("[PuzzleManager] No levels assigned!");
                return;
            }

            LoadLevel(currentLevelIndex);
        }

        private void Update()
        {
            // Check win condition
            if (!isLevelComplete && board != null && board.IsComplete())
            {
                OnLevelComplete();
            }
        }

        /// <summary>
        /// Load level by index
        /// </summary>
        public void LoadLevel(int index)
        {
            if (index < 0 || index >= levels.Length)
            {
                Debug.LogWarning($"[PuzzleManager] Invalid level index: {index}");
                return;
            }

            currentLevelIndex = index;
            LevelConfig level = levels[index];

            if (level == null)
            {
                Debug.LogError($"[PuzzleManager] Level {index} is null!");
                return;
            }

            // Initialize board in correct order
            if (board != null)
            {
                board.InitializeBoard();
                board.LoadLevel(level); // Load level first (sets currentLevel)
                board.SetTargetMap(level.GetTargetMap()); // Then set target map
                board.ShowTargetOverlay(); // Refresh overlay after setting map
            }

            isLevelComplete = false;

            Debug.Log($"[PuzzleManager] Loaded {level.LevelName} (Index: {index})");
        }

        /// <summary>
        /// Called when current level is completed
        /// </summary>
        private void OnLevelComplete()
        {
            isLevelComplete = true;

            Debug.Log($"[PuzzleManager] Level {currentLevelIndex + 1} Complete!");

            // TODO: Trigger win panel (Phase 8)
            // TODO: Play celebration effects (Phase 10)
        }

        /// <summary>
        /// Load next level
        /// </summary>
        public void NextLevel()
        {
            int nextIndex = currentLevelIndex + 1;

            if (nextIndex >= levels.Length)
            {
                Debug.Log("[PuzzleManager] All levels completed! Returning to main menu...");
                // TODO: Return to main menu (Phase 8)
                return;
            }

            LoadLevel(nextIndex);
        }

        /// <summary>
        /// Restart current level
        /// </summary>
        public void RestartLevel()
        {
            LoadLevel(currentLevelIndex);
        }

        /// <summary>
        /// Load specific level (for testing)
        /// </summary>
        public void LoadLevelByIndex(int index)
        {
            LoadLevel(index);
        }

        // Getters
        public int CurrentLevelIndex => currentLevelIndex;
        public int TotalLevels => levels != null ? levels.Length : 0;
        public bool IsLevelComplete => isLevelComplete;
        public LevelConfig CurrentLevel => (currentLevelIndex >= 0 && currentLevelIndex < levels.Length) ? levels[currentLevelIndex] : null;
    }
}