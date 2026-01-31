using UnityEngine;
using PuzzleGame.Gameplay.Board;
using PuzzleGame.Gameplay.Level;
using Game.Services.GameState;
using PuzzleGame.Gameplay.Timer;
using Game.Core;

namespace PuzzleGame.Gameplay
{
    /// <summary>
    /// Main manager for puzzle gameplay, level progression, and win/lose conditions
    /// </summary>
    public class PuzzleManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PuzzleBoard board;
        [SerializeField] private TimerService timerService; // NEW

        [Header("Levels")]
        [SerializeField] private LevelConfig[] levels;
        [SerializeField] private int currentLevelIndex = 0;

        private IGameStateService gameState;
        private bool isLevelComplete = false;

        private void Start()
        {
            // Get game state service
            gameState = ServiceLocator.Instance.Get<IGameStateService>();

            if (gameState == null)
            {
                Debug.LogError("[PuzzleManager] GameStateService not found!");
                return;
            }

            // Subscribe to timer events (NEW)
            if (timerService != null)
            {
                timerService.OnTimeUp += OnTimeUp;
            }

            // Load first level
            LoadLevel(currentLevelIndex);
        }

        private void OnDestroy()
        {
            // Unsubscribe from timer events (NEW)
            if (timerService != null)
            {
                timerService.OnTimeUp -= OnTimeUp;
            }
        }

        private void Update()
        {
            // Check for level completion
            if (!isLevelComplete && board != null && board.IsComplete())
            {
                OnLevelComplete();
            }
        }

        /// <summary>
        /// Load level by index
        /// </summary>
        public void LoadLevel(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= levels.Length)
            {
                Debug.LogWarning($"[PuzzleManager] Invalid level index: {levelIndex}");
                return;
            }

            currentLevelIndex = levelIndex;
            LevelConfig level = levels[currentLevelIndex];

            if (level == null)
            {
                Debug.LogError($"[PuzzleManager] Level {levelIndex} is null!");
                return;
            }

            // Reset state
            isLevelComplete = false;

            // Load level on board
            if (board != null)
            {
                board.InitializeBoard();
                board.LoadLevel(level);
            }

            // Start timer (NEW)
            if (timerService != null)
            {
                timerService.StartTimer(level.TimeLimit);
            }

            Debug.Log($"[PuzzleManager] Loaded level {levelIndex + 1}/{levels.Length}: {level.LevelName}");
        }

        /// <summary>
        /// Called when level is completed
        /// </summary>
        private void OnLevelComplete()
        {
            isLevelComplete = true;

            // Stop timer (NEW)
            if (timerService != null)
            {
                timerService.StopTimer();
            }

            // Trigger win state
            if (gameState != null)
            {
                gameState.TriggerWin();
            }

            Debug.Log($"[PuzzleManager] Level {currentLevelIndex + 1} completed!");
        }

        /// <summary>
        /// Called when timer runs out (NEW)
        /// </summary>
        private void OnTimeUp()
        {
            if (isLevelComplete)
                return; // Already won, ignore time up

            // Trigger lose state
            if (gameState != null)
            {
                gameState.TriggerLose();
            }

            Debug.Log("[PuzzleManager] Time's up! You lose.");
        }

        /// <summary>
        /// Load next level
        /// </summary>
        public void NextLevel()
        {
            int nextIndex = currentLevelIndex + 1;

            if (nextIndex < levels.Length)
            {
                LoadLevel(nextIndex);
            }
            else
            {
                // All levels completed, return to main menu
                Debug.Log("[PuzzleManager] All levels completed!");
                ReturnToMainMenu();
            }
        }

        /// <summary>
        /// Restart current level
        /// </summary>
        public void RestartLevel()
        {
            LoadLevel(currentLevelIndex);
        }

        /// <summary>
        /// Return to main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            var sceneLoader = ServiceLocator.Instance.Get<SceneLoader>();
            if (sceneLoader != null)
            {
                sceneLoader.LoadScene(SceneNames.MainMenu);
            }
        }

        // Getters
        public int CurrentLevelIndex => currentLevelIndex;
        public int TotalLevels => levels.Length;
        public LevelConfig CurrentLevel => levels[currentLevelIndex];
    }
}