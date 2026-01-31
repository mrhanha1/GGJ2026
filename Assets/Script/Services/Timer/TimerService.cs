using System;
using UnityEngine;

namespace PuzzleGame.Gameplay.Timer
{
    /// <summary>
    /// Service to manage game timer and countdown
    /// </summary>
    public class TimerService : MonoBehaviour
    {
        private float currentTime;
        private float timeLimit;
        private bool isRunning;
        private bool isPaused;

        // Events
        public event Action<float> OnTimeChanged; // Remaining time
        public event Action OnTimeUp;

        private void Update()
        {
            if (!isRunning || isPaused)
            {
                return;
            }

            currentTime -= Time.deltaTime;
            // Trigger time changed event
            OnTimeChanged?.Invoke(currentTime);

            // Check if time is up
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                isRunning = false;
                OnTimeUp?.Invoke();
                Debug.Log("[TimerService] Time's up!");
            }
        }

        /// <summary>
        /// Start timer with given time limit
        /// </summary>
        public void StartTimer(float limit)
        {
            timeLimit = limit;
            currentTime = limit;
            isRunning = true;
            isPaused = false;

            OnTimeChanged?.Invoke(currentTime);
            Debug.Log($"[TimerService] Timer started: {limit}s");
        }

        /// <summary>
        /// Pause timer
        /// </summary>
        public void PauseTimer()
        {
            if (!isRunning) return;

            isPaused = true;
            Debug.Log("[TimerService] Timer paused");
        }

        /// <summary>
        /// Resume timer
        /// </summary>
        public void ResumeTimer()
        {
            if (!isRunning) return;

            isPaused = false;
            Debug.Log("[TimerService] Timer resumed");
        }

        /// <summary>
        /// Stop timer
        /// </summary>
        public void StopTimer()
        {
            isRunning = false;
            isPaused = false;
            Debug.Log("[TimerService] Timer stopped");
        }

        /// <summary>
        /// Reset timer to initial time
        /// </summary>
        public void ResetTimer()
        {
            currentTime = timeLimit;
            OnTimeChanged?.Invoke(currentTime);
        }

        // Getters
        public float RemainingTime => currentTime;
        public float TimeLimit => timeLimit;
        public bool IsRunning => isRunning;
        public bool IsPaused => isPaused;
    }
}