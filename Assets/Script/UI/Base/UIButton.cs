// Assets/_Project/Scripts/UI/Base/UIButton.cs

using UnityEngine;
using UnityEngine.UI;
using Game.Core;
using Game.Services.Audio;

namespace Game.UI
{
    /// <summary>
    /// UI button wrapper with audio feedback.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioData audioData;

        private Button _button;
        private IAudioService _audioService;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (_audioService == null)
                _audioService = ServiceLocator.Instance.Get<IAudioService>();

            if (_audioService != null && audioData != null && audioData.buttonClickSFX != null)
            {
                _audioService.PlaySFX(audioData.buttonClickSFX);
            }
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }
    }
}