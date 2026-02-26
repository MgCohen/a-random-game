using CardMatch.CardMatch;
using CardMatch.Navigation;

namespace CardMatch.PlaySystem
{
    public class PlayViewContext : IViewContext
    {
        public Match Match { get; }
        public Level Level { get; }
        public IPlaySystem PlaySystem { get; }

        public PlayViewContext(Match match, Level level, IPlaySystem playSystem)
        {
            Match = match;
            Level = level;
            PlaySystem = playSystem;
        }

        public void CompleteMatch()
        {
            PlaySystem.MarkCompleted(Level);
        }

        public void GoBack()
        {
            PlaySystem.GoBack();
        }
    }
}
