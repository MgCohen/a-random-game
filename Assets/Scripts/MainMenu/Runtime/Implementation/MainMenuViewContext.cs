using System.Collections.Generic;
using CardMatch.CardMatch;
using CardMatch.Levels;
using CardMatch.Navigation;
using CardMatch.PlayView;

namespace CardMatch.MainMenu
{
    public class MainMenuViewContext : IViewContext
    {
        private readonly ILevelController levelController;
        private readonly INavigation navigation;
        private Level selectedLevel;

        public Level SelectedLevel => selectedLevel;

        public MainMenuViewContext(ILevelController levelController, INavigation navigation)
        {
            this.levelController = levelController;
            this.navigation = navigation;
        }

        public IReadOnlyList<MainMenuLevelEntry> GetLevelEntries()
        {
            IReadOnlyList<Level> levels = levelController.GetLevels();
            var entries = new List<MainMenuLevelEntry>(levels.Count);
            for (int i = 0; i < levels.Count; i++)
            {
                Level level = levels[i];
                if (level == null) continue;
                entries.Add(CreateEntry(level));
            }
            return entries;
        }

        public bool CanSelect(Level level)
        {
            if (level == null) return false;
            return levelController.IsUnlocked(level) || levelController.IsCompleted(level);
        }

        public void SelectLevel(Level level)
        {
            if (!CanSelect(level)) return;
            selectedLevel = level;
        }

        public void OnSettingsClicked()
        {
        }

        public void OnPlayClicked()
        {
            if (navigation == null) return;
            navigation.Open(new PlayViewContext(navigation));
        }

        private MainMenuLevelEntry CreateEntry(Level level)
        {
            LevelProgressState state = GetProgressState(level);
            return new MainMenuLevelEntry(level, state);
        }

        private LevelProgressState GetProgressState(Level level)
        {
            if (levelController.IsCompleted(level)) return LevelProgressState.Completed;
            if (levelController.IsUnlocked(level)) return LevelProgressState.Unlocked;
            return LevelProgressState.Locked;
        }
    }
}
