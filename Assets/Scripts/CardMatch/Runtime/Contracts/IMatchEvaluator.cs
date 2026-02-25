using CardMatch.Config;

namespace CardMatch.CardMatch
{
    public interface IMatchEvaluator
    {
        bool IsCompleted { get; }
        void Evaluate(GameState state);
    }
}
