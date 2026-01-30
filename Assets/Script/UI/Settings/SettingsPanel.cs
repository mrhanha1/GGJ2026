// Assets/_Project/Scripts/UI/Settings/SettingsPanel.cs

using UnityEngine;
using UnityEngine.UI;
using Game.Core;
using Game.Services.Audio;

namespace Game.UI
{
    /// <summary>
    /// Settings panel with volume controls.
    /// </summary>
    public class SettingsPanel : UIPanel
    {
        [Header("Sliders")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;

        private IAudioService _audioService;

        protected override void Awake()
        {
            base.Awake();

            _audioService = ServiceLocator.Instance.Get<IAudioService>();

            // Load current volumes
            if (_audioService != null)
            {
                masterSlider.value = _audioService.GetMasterVolume();
                musicSlider.value = _audioService.GetMusicVolume();
                sfxSlider.value = _audioService.GetSFXVolume();
            }

            // Add listeners
            masterSlider.onValueChanged.AddListener(OnMasterChanged);
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
            closeButton.onClick.AddListener(Hide);
        }

        private void OnMasterChanged(float value)
        {
            _audioService?.SetMasterVolume(value);
        }

        private void OnMusicChanged(float value)
        {
            _audioService?.SetMusicVolume(value);
        }

        private void OnSFXChanged(float value)
        {
            _audioService?.SetSFXVolume(value);
        }

        private void OnDestroy()
        {
            masterSlider.onValueChanged.RemoveListener(OnMasterChanged);
            musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
            sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
            closeButton.onClick.RemoveListener(Hide);
        }
    }
}