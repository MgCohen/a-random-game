using System.Collections.Generic;
using CardMatch.CardMatch;

namespace CardMatch.Levels
{
    public class LevelProgressSaveHandler
    {
        public LevelProgressSave ToPersistence(LevelCompletionState state)
        {
            IReadOnlyDictionary<string, LevelProgressState> snapshot = state.GetSnapshot();
            var entries = SnapshotToEntries(snapshot);
            return new LevelProgressSave { Entries = entries.ToArray() };
        }

        private static List<LevelProgressEntry> SnapshotToEntries(IReadOnlyDictionary<string, LevelProgressState> snapshot)
        {
            var entries = new List<LevelProgressEntry>();
            foreach (KeyValuePair<string, LevelProgressState> kv in snapshot)
            {
                entries.Add(new LevelProgressEntry { LevelId = kv.Key, State = (int)kv.Value });
            }
            return entries;
        }

        public LevelCompletionState ToState(LevelProgressSave save, IReadOnlyList<Level> levels)
        {
            var state = new LevelCompletionState();
            Dictionary<string, int> saveByLevelId = BuildSaveLookup(save);
            ApplyLevelsToState(state, levels, saveByLevelId);
            return state;
        }

        private Dictionary<string, int> BuildSaveLookup(LevelProgressSave save)
        {
            var lookup = new Dictionary<string, int>();
            if (save?.Entries == null) return lookup;
            foreach (LevelProgressEntry entry in save.Entries)
            {
                TryAddEntryToLookup(lookup, entry);
            }
            return lookup;
        }

        private static void TryAddEntryToLookup(Dictionary<string, int> lookup, LevelProgressEntry entry)
        {
            if (entry == null || string.IsNullOrEmpty(entry.LevelId)) return;
            lookup[entry.LevelId] = entry.State;
        }

        private static void ApplyLevelsToState(LevelCompletionState state, IReadOnlyList<Level> levels, Dictionary<string, int> saveByLevelId)
        {
            if (levels == null) return;
            for (int i = 0; i < levels.Count; i++)
            {
                Level level = levels[i];
                if (level == null) continue;
                LevelProgressState progressState = GetInitialProgressState(level.LevelId, i, saveByLevelId);
                state.SetState(level.LevelId, progressState);
            }
        }

        private static LevelProgressState GetInitialProgressState(string levelId, int index, Dictionary<string, int> saveByLevelId)
        {
            if (saveByLevelId.TryGetValue(levelId, out int savedState)) return (LevelProgressState)savedState;
            if (index == 0) return LevelProgressState.Unlocked;
            return LevelProgressState.Locked;
        }
    }
}
