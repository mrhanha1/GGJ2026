// Assets/_Project/Scripts/Services/Audio/IAudioService.cs

using UnityEngine;

namespace Game.Services.Audio
{
    /// <summary>
    /// Interface for audio management service.
    /// Handles music playback, SFX, and volume control.
    /// </summary>
    public interface IAudioService : Game.Core.IService
    {
        // Music
        void PlayMusic(AudioClip clip, bool loop = true);
        void StopMusic();
        void SetMusicVolume(float volume);
        float GetMusicVolume();

        // SFX
        void PlaySFX(AudioClip clip);
        void PlaySFX(AudioClip clip, float volume);
        void SetSFXVolume(float volume);
        float GetSFXVolume();

        // Master
        void SetMasterVolume(float volume);
        float GetMasterVolume();
    }
}