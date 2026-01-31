using UnityEngine;
using PuzzleGame.Gameplay.Board;
using PuzzleGame.Gameplay.Level;
using Game.Services.GameState;
using PuzzleGame.Gameplay.Timer;
using Game.Core;
using UnityEngine.UI;

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
        [SerializeField] private Animator mascotAnimator; // NEW: Mascot animator for win animations
        [SerializeField] private Game.UI.WinPanel winPanel; // NEW: Win panel to show after animation

        [Header("Levels")]
        [SerializeField] private LevelConfig[] levels;
        [SerializeField] private int currentLevelIndex = 0;

        [Header("Animation Settings")]
        [SerializeField] private float winAnimationDelay = 3f; // Delay before showing win panel
        [SerializeField] private Button skiplevelbutton;

        private IGameStateService gameState;
        private bool isLevelComplete = false;
        private Coroutine winAnimationCoroutine; // Track win animation coroutine

        private void Start()
        {
            // Get game state service
            gameState = ServiceLocator.Instance.Get<IGameStateService>();

            if (gameState == null)
            {
                Debug.LogError("[PuzzleManager] GameStateService not found!");
                return;
            }
            gameState.SetState(GameState.Playing);

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

            // Reset mascot animator win parameter to -1
            if (mascotAnimator != null)
            {
                mascotAnimator.SetInteger("win", -1);
                Debug.Log("[PuzzleManager] Reset mascot win parameter to -1");
            }

            // Stop any ongoing win animation coroutine
            if (winAnimationCoroutine != null)
            {
                StopCoroutine(winAnimationCoroutine);
                winAnimationCoroutine = null;
            }

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

            // Stop timer
            if (timerService != null)
            {
                timerService.StopTimer();
            }

            // Set mascot animator win parameter to current level index
            if (mascotAnimator != null)
            {
                mascotAnimator.SetInteger("win", currentLevelIndex);
                Debug.Log($"[PuzzleManager] Set mascot win parameter to {currentLevelIndex}");
            }

            // Start coroutine to delay win panel
            winAnimationCoroutine = StartCoroutine(ShowWinPanelAfterDelay());

            Debug.Log($"[PuzzleManager] Level {currentLevelIndex + 1} completed!");
        }
        public void ForceCompleteLevel()
        {
            if (isLevelComplete)
                return;

            OnLevelComplete();
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
        /// Coroutine to show win panel after animation delay
        /// </summary>
        private System.Collections.IEnumerator ShowWinPanelAfterDelay()
        {
            // Wait for animation delay (3 seconds)
            yield return new UnityEngine.WaitForSeconds(winAnimationDelay);

            // Trigger win state
            if (gameState != null)
            {
                gameState.TriggerWin();
            }

            // Show win panel
            if (winPanel != null)
            {
                winPanel.Show();
                Debug.Log("[PuzzleManager] Win panel shown after animation");
            }

            winAnimationCoroutine = null;
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