using System.Collections.Generic;
using UnityEngine;

namespace CardMatch.Audio
{
    public class AudioService : MonoBehaviour, IAudioService
    {
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private int maxConcurrentSounds = 5;

        private List<AudioSource> _pool = new List<AudioSource>();

        private void Awake()
        {
            CreatePool();
        }

        private void CreatePool()
        {
            if (sfxSource == null)
                return;
            SpawnPool();
        }

        private void SpawnPool()
        {
            int limit = Mathf.Max(1, maxConcurrentSounds);
            _pool.Add(sfxSource);
            for (int i = 1; i < limit; i++)
            {
                SpawnSource();
            }
        }

        private void SpawnSource()
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = sfxSource.spatialBlend;
            source.volume = sfxSource.volume;
            source.mute = sfxSource.mute;
            source.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
            _pool.Add(source);
        }

        public bool IsMuted
        {
            get
            {
                if (_pool == null || _pool.Count == 0)
                    return sfxSource != null && sfxSource.mute;
                return _pool[0].mute;
            }
        }

        public void PlaySound(AudioClip clip)
        {
            if (clip == null)
                return;
            if (_pool == null || _pool.Count == 0)
                return;
            AudioSource availableSource = GetFirstAvailablePool();
            if(availableSource == null)
            {
                return;
            }
            PlaySound(availableSource, clip);
        }

        private AudioSource GetFirstAvailablePool()
        {
            foreach (var source in _pool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }
            return null;
        }

        private void PlaySound(AudioSource source, AudioClip clip)
        {
            source.clip = clip;
            source.Play();
        }

        public void SetMute(bool mute)
        {
            foreach(var source in _pool)
            {
                source.mute = mute;
            }
        }
    }
}
