using System;

namespace CardMatch.Levels
{
    [Serializable]
    public class LevelProgressSave
    {
        public LevelProgressEntry[] Entries = Array.Empty<LevelProgressEntry>();
    }

    [Serializable]
    public class LevelProgressEntry
    {
        public string LevelId;
        public int State;
    }
}
