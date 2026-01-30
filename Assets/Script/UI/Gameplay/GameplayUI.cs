using UnityEngine;
using Game.Core;
using Game.Services.GameState;

namespace Game.UI
{
    /// <summary>
    /// Gameplay UI controller.
    /// </summary>
    public class GameplayUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private WinPanel winPanel;
        [SerializeField] private LosePanel losePanel;

        private IGameStateService _gameStateService;

        private void Start()
        {
            _gameStateService = ServiceLocator.Instance.Get<IGameStateService>();

            if (_gameStateService != null)
            {
                _gameStateService.OnWin += OnWin;
                _gameStateService.OnLose += OnLose;
            }

            // Hide panels initially
            if (winPanel != null)
                winPanel.gameObject.SetActive(false);
            if (losePanel != null)
                losePanel.gameObject.SetActive(false);
        }

        private void OnWin()
        {
            if (winPanel != null)
                winPanel.Show();
        }

        private void OnLose()
        {
            if (losePanel != null)
                losePanel.Show();
        }

        private void OnDestroy()
        {
            if (_gameStateService != null)
            {
                _gameStateService.OnWin -= OnWin;
                _gameStateService.OnLose -= OnLose;
            }
        }
    }
}