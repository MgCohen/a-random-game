using UnityEngine;

namespace CardMatch.Audio
{
    public interface IAudioService
    {
        bool IsMuted { get; }
        void SetMute(bool mute);
        void PlaySound(AudioClip clip);
    }
}
