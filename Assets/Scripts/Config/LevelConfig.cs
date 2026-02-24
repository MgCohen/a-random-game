using System;

namespace CardMatch
{
    [Serializable]
    public class LevelConfig
    {
        public LayoutConfig Layout = new LayoutConfig();
        public ScoreRules Scoring = new ScoreRules();
    }
}
