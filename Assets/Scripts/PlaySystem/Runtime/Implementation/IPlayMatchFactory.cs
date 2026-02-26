using CardMatch.CardMatch;

namespace CardMatch.PlaySystem
{
    public interface IPlayMatchFactory
    {
        Match Create(Level level);
    }
}
