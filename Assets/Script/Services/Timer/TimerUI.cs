using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGame.UI.Gameplay
{
    /// <summary>
    /// UI display for game timer
    /// </summary>
    public class TimerUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Text timerText;
        [SerializeField] private PuzzleGame.Gameplay.Timer.TimerService timerService;

        [Header("Warning Settings")]
        [SerializeField] private float warningThreshold = 10f; // seconds
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color warningColor = Color.red;

        private void Start()
        {
            if (timerService == null)
            {
                Debug.LogError("[TimerUI] TimerService not assigned!");
                return;
            }

            // Subscribe to timer events
            timerService.OnTimeChanged += UpdateDisplay;

            // Initial display
            UpdateDisplay(timerService.TimeLimit);
        }

        private void OnDestroy()
        {
            if (timerService != null)
            {
                timerService.OnTimeChanged -= UpdateDisplay;
            }
        }

        /// <summary>
        /// Update timer display with MM:SS format
        /// </summary>
        private void UpdateDisplay(float remainingTime)
        {
            if (timerText == null)
                return;

            // Convert to minutes and seconds
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);

            // Format as MM:SS
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Change color if time is low
            if (remainingTime <= warningThreshold)
            {
                timerText.color = warningColor;
            }
            else
            {
                timerText.color = normalColor;
            }
        }
    }
}