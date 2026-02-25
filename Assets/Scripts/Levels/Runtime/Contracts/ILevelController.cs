using System.Collections.Generic;
using CardMatch.CardMatch;

namespace CardMatch.Levels
{
    public interface ILevelController
    {
        IReadOnlyList<Level> GetLevels();
        Level GetLevel(int index);
        bool IsUnlocked(Level level);
        bool IsCompleted(Level level);
        void MarkCompleted(Level level);
    }
}
