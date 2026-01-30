// Assets/_Project/Scripts/Services/Audio/AudioService.cs

using System.Collections.Generic;
using UnityEngine;

namespace Game.Services.Audio
{
    /// <summary>
    /// Audio service implementation with music and SFX management.
    /// Creates AudioSources at runtime and manages volume via PlayerPrefs.
    /// </summary>
    public class AudioService : MonoBehaviour, IAudioService
    {
        private const string MASTER_VOLUME_KEY = "Audio_MasterVolume";
        private const string MUSIC_VOLUME_KEY = "Audio_MusicVolume";
        private const string SFX_VOLUME_KEY = "Audio_SFXVolume";

        [Header("Settings")]
        [SerializeField] private int initialSFXPoolSize = 5;

        private AudioSource _musicSource;
        private List<AudioSource> _sfxPool = new List<AudioSource>();

        private float _masterVolume = 1f;
        private float _musicVolume = 1f;
        private float _sfxVolume = 1f;

        public void Initialize()
        {
            // Create music source
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;

            // Create SFX pool
            for (int i = 0; i < initialSFXPoolSize; i++)
            {
                CreateSFXSource();
            }

            // Load saved volumes
            LoadVolumes();

            Debug.Log($"[AudioService] Initialized with {initialSFXPoolSize} SFX sources.");
        }

        private AudioSource CreateSFXSource()
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.loop = false;
            source.playOnAwake = false;
            _sfxPool.Add(source);
            return source;
        }

        // ===== MUSIC =====
        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioService] Music clip is null.");
                return;
            }

            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.volume = _musicVolume * _masterVolume;
            _musicSource.Play();
            Debug.Log($"[AudioService] Playing music: {clip.name}");
        }

        public void StopMusic()
        {
            _musicSource.Stop();
            Debug.Log("[AudioService] Music stopped.");
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            _musicSource.volume = _musicVolume * _masterVolume;
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, _musicVolume);
            PlayerPrefs.Save();
        }

        public float GetMusicVolume() => _musicVolume;

        // ===== SFX =====
        public void PlaySFX(AudioClip clip)
        {
            PlaySFX(clip, 1f);
        }

        public void PlaySFX(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioService] SFX clip is null.");
                return;
            }

            AudioSource source = GetAvailableSFXSource();
            source.volume = volume * _sfxVolume * _masterVolume;
            source.PlayOneShot(clip);
        }

        private AudioSource GetAvailableSFXSource()
        {
            // Find available source
            foreach (var source in _sfxPool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            // All busy, expand pool
            Debug.Log("[AudioService] SFX pool full, expanding pool.");
            return CreateSFXSource();
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, _sfxVolume);
            PlayerPrefs.Save();
        }

        public float GetSFXVolume() => _sfxVolume;

        // ===== MASTER =====
        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            _musicSource.volume = _musicVolume * _masterVolume;
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, _masterVolume);
            PlayerPrefs.Save();
        }

        public float GetMasterVolume() => _masterVolume;

        // ===== SAVE/LOAD =====
        private void LoadVolumes()
        {
            _masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
            _musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
            _sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

            _musicSource.volume = _musicVolume * _masterVolume;

            Debug.Log($"[AudioService] Loaded volumes - Master: {_masterVolume}, Music: {_musicVolume}, SFX: {_sfxVolume}");
        }
    }
}