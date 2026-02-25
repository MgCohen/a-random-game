using UnityEngine;

namespace CardMatch.Audio
{
    public class AudioService : MonoBehaviour, IAudioService
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        public bool IsMuted
        {
            get
            {
                return musicSource != null && musicSource.mute;
            }
        }

        public void PlaySound(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
                sfxSource.PlayOneShot(clip);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource == null) return;
            musicSource.Stop();
            if (clip == null) return;
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void StopMusic()
        {
            if (musicSource != null)
                musicSource.Stop();
        }

        public void SetMusicVolume(float volume)
        {
            if (musicSource != null)
                musicSource.volume = Mathf.Clamp01(volume);
        }

        public void SetMute(bool mute)
        {
            if (musicSource != null)
            {
                musicSource.mute = mute;
            }
            if (sfxSource != null)
            {
                sfxSource.mute = mute;
            }
        }
    }
}
