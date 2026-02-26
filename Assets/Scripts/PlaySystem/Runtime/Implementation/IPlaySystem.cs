using CardMatch.CardMatch;

namespace CardMatch.PlaySystem
{
    public interface IPlaySystem
    {
        void Play(Level level);
        void GoBack();
        void MarkCompleted(Level level);
    }
}
