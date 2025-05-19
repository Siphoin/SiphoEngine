using SFML.Audio;
using System;
using System.Collections.Generic;

namespace SiphoEngine.Core.Audio
{
    public static class AudioEngine
    {
        private static readonly Dictionary<string, SoundBuffer> _soundBuffers = new();
        private static readonly List<Sound> _activeSounds = new();
        private static Music _currentMusic;
        private static float _globalVolume = 100f;
        private static bool _isMuted = false;

        public static float GlobalVolume
        {
            get => _globalVolume;
            set
            {
                _globalVolume = Math.Clamp(value, 0, 100);
                UpdateVolumes();
            }
        }

        public static void LoadSound(string name, string filePath)
        {
            if (_soundBuffers.ContainsKey(name))
                throw new ArgumentException($"Sound '{name}' already loaded");

            _soundBuffers[name] = new SoundBuffer(filePath);
        }

        public static Sound PlaySound(string name, float volume = 100f, bool loop = false)
        {
            if (!_soundBuffers.TryGetValue(name, out var buffer))
                throw new ArgumentException($"Sound '{name}' not found");

            var sound = new Sound(buffer)
            {
                Volume = _isMuted ? 0 : volume * (_globalVolume / 100),
                Loop = loop
            };

            sound.Play();
            _activeSounds.Add(sound);

            return sound;
        }

        public static void PlayMusic(string filePath, float volume = 100f, bool loop = true)
        {
            StopMusic();

            _currentMusic = new Music(filePath)
            {
                Volume = _isMuted ? 0 : volume * (_globalVolume / 100),
                Loop = loop
            };

            _currentMusic.Play();
        }

        public static void StopMusic()
        {
            _currentMusic?.Stop();
            _currentMusic?.Dispose();
            _currentMusic = null;
        }

        public static void PauseMusic()
        {
            _currentMusic?.Pause();
        }

        public static void ResumeMusic()
        {
            _currentMusic?.Play();
        }

        public static void SetMute(bool mute)
        {
            _isMuted = mute;
            UpdateVolumes();
        }

        public static void StopAllSounds()
        {
            foreach (var sound in _activeSounds)
            {
                sound.Stop();
                sound.Dispose();
            }
            _activeSounds.Clear();
        }

       internal static void Update()
        {
            for (int i = _activeSounds.Count - 1; i >= 0; i--)
            {
                if (_activeSounds[i].Status != SoundStatus.Playing && !_activeSounds[i].Loop)
                {
                    _activeSounds[i].Dispose();
                    _activeSounds.RemoveAt(i);
                }
            }
        }

        private static void UpdateVolumes()
        {
            float effectiveVolume = _isMuted ? 0 : _globalVolume;

            foreach (var sound in _activeSounds)
            {
                sound.Volume = sound.Volume * (effectiveVolume / 100);
            }

            if (_currentMusic != null)
            {
                _currentMusic.Volume = _currentMusic.Volume * (effectiveVolume / 100);
            }
        }

        public static void Cleanup()
        {
            StopAllSounds();
            StopMusic();

            foreach (var buffer in _soundBuffers.Values)
            {
                buffer.Dispose();
            }
            _soundBuffers.Clear();
        }
    }
}