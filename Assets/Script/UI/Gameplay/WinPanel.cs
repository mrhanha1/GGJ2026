// Assets/_Project/Scripts/UI/Gameplay/WinPanel.cs

using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.UI
{
    /// <summary>
    /// Win panel UI.
    /// </summary>
    public class WinPanel : UIPanel
    {
        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private SceneLoader _sceneLoader;

        protected override void Awake()
        {
            base.Awake();

            _sceneLoader = ServiceLocator.Instance.Get<SceneLoader>();

            restartButton.onClick.AddListener(OnRestartClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        private void OnRestartClicked()
        {
            _sceneLoader.LoadScene(SceneNames.Gameplay);
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