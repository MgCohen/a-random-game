using System;

namespace CardMatch.CardMatch
{
    [Serializable]
    public class LevelConfig
    {
        public LayoutConfig Layout = new LayoutConfig();
        public ScoreRules Scoring = new ScoreRules();
    }
}
