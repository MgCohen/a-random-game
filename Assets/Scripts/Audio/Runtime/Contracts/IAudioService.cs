namespace CardMatch.Audio
{
    public interface IAudioService
    {
        bool IsMuted { get; }
        void SetMute(bool mute);
    }
}
