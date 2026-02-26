using CardMatch.CardMatch;
using CardMatch.Levels;
using CardMatch.Navigation;

namespace CardMatch.PlaySystem
{
    public class PlaySystem : IPlaySystem
    {
        private readonly INavigation navigation;
        private readonly IPlayMatchFactory factory;
        private readonly ILevelController levelController;

        public PlaySystem(INavigation navigation, IPlayMatchFactory factory, ILevelController levelController)
        {
            this.navigation = navigation;
            this.factory = factory;
            this.levelController = levelController;
        }

        public void Play(Level level)
        {
            if (navigation == null || level == null) return;
            Match match = factory.Create(level);
            navigation.Open(new PlayViewContext(match, level, this), true);
        }

        public void GoBack()
        {
            if (navigation == null) return;
            navigation.GoBack();
        }

        public void MarkCompleted(Level level)
        {
            if (levelController == null || level == null) return;
            levelController.MarkCompleted(level);
        }
    }
}
