using System.Collections.Generic;
using CardMatch.CardMatch;
using UnityEngine;

namespace CardMatch.Levels
{
    public class LevelController : ILevelController
    {
        private readonly LevelRegistry registry;
        private readonly LevelCompletionState state;

        public LevelController(LevelRegistry registry, LevelCompletionState state)
        {
            this.registry = registry;
            this.state = state;
        }

        public IReadOnlyList<Level> GetLevels()
        {
            if (registry == null || registry.Levels == null)
            {
                return new List<Level>();
            }
            return registry.Levels;
        }

        public Level GetLevel(int index)
        {
            IReadOnlyList<Level> list = GetLevels();
            if (index < 0 || index >= list.Count)
            {
                return null;
            }
            return list[index];
        }

        public bool IsUnlocked(Level level)
        {
            if (level == null)
            {
                return false;
            }
            LevelProgressState progressState = state.GetState(level.LevelId);
            return progressState == LevelProgressState.Unlocked || progressState == LevelProgressState.Completed;
        }

        public bool IsCompleted(Level level)
        {
            if (level == null)
            {
                return false;
            }
            return state.GetState(level.LevelId) == LevelProgressState.Completed;
        }

        public void MarkCompleted(Level level)
        {
            if (level == null) return;
            state.SetState(level.LevelId, LevelProgressState.Completed);
            UnlockNextLevelIfAny(level);
        }

        private void UnlockNextLevelIfAny(Level level)
        {
            Level nextLevel = GetNextLevel(level);
            if (nextLevel == null) return;
            state.SetState(nextLevel.LevelId, LevelProgressState.Unlocked);
        }

        private Level GetNextLevel(Level level)
        {
            int index = IndexOf(level);
            if (index < 0) return null;
            return GetLevel(index + 1);
        }

        private int IndexOf(Level level)
        {
            if (level == null || registry?.Levels == null) return -1;
            return FindLevelIndex(registry.Levels, level);
        }

        private static int FindLevelIndex(Level[] levels, Level level)
        {
            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i] == level) return i;
            }
            return -1;
        }
    }
}
