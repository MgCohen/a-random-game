using System;
using System.Collections.Generic;
using CardMatch.CardMatch;
using CardMatch.Persistence;
using UnityEngine;

namespace CardMatch.Levels
{
    public class LevelController : ILevelController
    {
        private readonly LevelRegistry registry;
        private readonly Dictionary<string, LevelProgressState> stateCache = new Dictionary<string, LevelProgressState>();
        private readonly IPersistence persistence;

        public LevelController(LevelRegistry registry, IPersistence persistence)
        {
            this.registry = registry;
            this.persistence = persistence;
            Level[] levels = registry?.Levels;
            LevelCompletionState loaded = persistence?.Load<LevelCompletionState>();

            if (loaded != null && (loaded.UnlockedLevelIds.Count > 0 || loaded.CompletedLevelIds.Count > 0))
            {
                foreach (string id in loaded.CompletedLevelIds)
                {
                    if (!string.IsNullOrEmpty(id))
                        stateCache[id] = LevelProgressState.Completed;
                }
                foreach (string id in loaded.UnlockedLevelIds)
                {
                    if (!string.IsNullOrEmpty(id) && !stateCache.ContainsKey(id))
                        stateCache[id] = LevelProgressState.Unlocked;
                }
            }
            else
            {
                if (levels != null && levels.Length > 0 && !string.IsNullOrEmpty(levels[0].LevelId))
                    stateCache[levels[0].LevelId] = LevelProgressState.Unlocked;
            }
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
            return stateCache.TryGetValue(level.LevelId, out LevelProgressState pState)
                && (pState == LevelProgressState.Unlocked || pState == LevelProgressState.Completed);
        }

        public bool IsCompleted(Level level)
        {
            if (level == null)
            {
                return false;
            }
            return stateCache.TryGetValue(level.LevelId, out LevelProgressState pState)
                && pState == LevelProgressState.Completed;
        }

        public void MarkCompleted(Level level)
        {
            if (level == null) return;
            stateCache[level.LevelId] = LevelProgressState.Completed;
            Level nextLevel = GetNextLevel(level);
            if (nextLevel != null && !IsUnlocked(nextLevel))
            {
                stateCache[nextLevel.LevelId] = LevelProgressState.Unlocked;
            }
            LevelCompletionState toSave = BuildCompletionState();
            persistence?.Save(toSave);
        }

        private LevelCompletionState BuildCompletionState()
        {
            var toSave = new LevelCompletionState();
            foreach (KeyValuePair<string, LevelProgressState> kv in stateCache)
            {
                if (kv.Value == LevelProgressState.Unlocked)
                    toSave.UnlockedLevelIds.Add(kv.Key);
                else if (kv.Value == LevelProgressState.Completed)
                    toSave.CompletedLevelIds.Add(kv.Key);
            }
            return toSave;
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
