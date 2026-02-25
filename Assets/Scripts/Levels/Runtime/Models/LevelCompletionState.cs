using System.Collections.Generic;
using CardMatch.CardMatch;

namespace CardMatch.Levels
{
    public class LevelCompletionState
    {
        private readonly Dictionary<string, LevelProgressState> stateByLevelId = new Dictionary<string, LevelProgressState>();

        public LevelProgressState GetState(string levelId)
        {
            if (string.IsNullOrEmpty(levelId))
            {
                return LevelProgressState.Locked;
            }
            if (stateByLevelId.TryGetValue(levelId, out LevelProgressState state))
            {
                return state;
            }
            return LevelProgressState.Locked;
        }

        public void SetState(string levelId, LevelProgressState state)
        {
            if (string.IsNullOrEmpty(levelId))
            {
                return;
            }
            stateByLevelId[levelId] = state;
        }

        internal IReadOnlyDictionary<string, LevelProgressState> GetSnapshot()
        {
            var snapshot = new Dictionary<string, LevelProgressState>(stateByLevelId);
            return snapshot;
        }
    }
}
