// Assets/_Project/Scripts/Services/Audio/AudioData.cs

using UnityEngine;

namespace Game.Services.Audio
{
    /// <summary>
    /// ScriptableObject containing audio clips and settings.
    /// Create via: Assets → Create → Game → Audio Data
    /// </summary>
    [CreateAssetMenu(fileName = "AudioData", menuName = "Game/Audio Data", order = 1)]
    public class AudioData : ScriptableObject
    {
        [Header("Music")]
        public AudioClip mainMenuMusic;
        public AudioClip gameplayMusic;

        [Header("SFX")]
        public AudioClip buttonClickSFX;
        public AudioClip winSFX;
        public AudioClip loseSFX;
        public AudioClip explosionSFX;

        // Add more clips as needed
    }
}