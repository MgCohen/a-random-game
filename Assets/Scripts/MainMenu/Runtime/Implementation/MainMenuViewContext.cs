using System.Collections.Generic;
using CardMatch.Audio;
using CardMatch.CardMatch;
using CardMatch.Levels;
using CardMatch.Navigation;
using CardMatch.PlaySystem;
using CardMatch.Settings;

namespace CardMatch.MainMenu
{
    public class MainMenuViewContext : IViewContext
    {
        private readonly ILevelController levelController;
        private readonly INavigation navigation;
        private readonly IPlaySystem playSystem;
        private readonly IAudioService audioService;
        private Level selectedLevel;

        public Level SelectedLevel => selectedLevel;

        public MainMenuViewContext(ILevelController levelController, INavigation navigation, IPlaySystem playSystem, IAudioService audioService)
        {
            this.levelController = levelController;
            this.navigation = navigation;
            this.playSystem = playSystem;
            this.audioService = audioService;
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
            PlaySelected();
        }

        public void OnSettingsClicked()
        {
            if (navigation == null || audioService == null) return;
            var settingsContext = new SettingsViewContext(navigation, audioService);
            navigation.Open(settingsContext);
        }

        public void PlaySelected()
        {
            if (playSystem == null) return;
            if (selectedLevel == null) return;
            playSystem.Play(selectedLevel);
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
