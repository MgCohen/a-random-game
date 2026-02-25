using CardMatch.Config;

namespace CardMatch.CardMatch
{
    public class ScoreService
    {
        private GameState state;
        private ScoreRules rules;
        private IMatchEvents events;

        public ScoreService(GameState gameState, ScoreRules scoreRules, IMatchEvents matchEvents)
        {
            state = gameState;
            rules = scoreRules;
            events = matchEvents;
            events.Subscribe<CardsMatched>(OnCardsMatched);
            events.Subscribe<CardsMismatched>(OnCardsMismatched);
        }

        public void Dispose()
        {
            events.Unsubscribe<CardsMatched>(OnCardsMatched);
            events.Unsubscribe<CardsMismatched>(OnCardsMismatched);
        }

        private void OnCardsMatched(CardsMatched matchedEvent)
        {
            int pointsToAdd = CalculatePointsToAdd(state.Combo);
            int newCombo = state.Combo + 1;
            state.Score += pointsToAdd;
            state.Combo = newCombo;
            events.Publish(new ScoreChanged(state.Score, pointsToAdd));
            events.Publish(new ComboChanged(newCombo));
        }

        private void OnCardsMismatched(CardsMismatched mismatchedEvent)
        {
            if (state.Combo == 0)
            {
                return;
            }
            state.Combo = 0;
            events.Publish(new ComboChanged(0));
        }

        private int CalculatePointsToAdd(int currentCombo)
        {
            int comboBonus = currentCombo * rules.ComboBonusPerLevel;
            return rules.BaseMatchPoints + comboBonus;
        }
    }
}
