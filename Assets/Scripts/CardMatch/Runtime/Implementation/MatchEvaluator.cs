using System.Collections.Generic;
using CardMatch.Config;

namespace CardMatch.CardMatch
{
    public class MatchEvaluator : IMatchEvaluator
    {
        private readonly IMatchEvents events;
        private bool isCompleted;

        public bool IsCompleted => isCompleted;

        public MatchEvaluator(IMatchEvents matchEvents)
        {
            events = matchEvents;
        }

        public void Evaluate(GameState state)
        {
            UpdateCompletedFromState(state);
            if (state.FlippedCards == null || state.FlippedCards.Count < 2)
            {
                return;
            }
            Card first = state.FlippedCards[0];
            Card second = state.FlippedCards[1];
            if (first.CardId == second.CardId)
            {
                ApplyMatch(state, first, second);
            }
            else
            {
                ApplyMismatch(state, first, second);
            }
        }

        protected virtual void ApplyMatch(GameState state, Card first, Card second)
        {
            SetCardStateAndDispatch(first, CardState.Scored);
            SetCardStateAndDispatch(second, CardState.Scored);
            state.FlippedCards.Clear();
            state.Round += 1;
            events.Publish(new CardsMatched(first, second));
            UpdateCompletedState(state);
        }

        protected virtual void ApplyMismatch(GameState state, Card first, Card second)
        {
            SetCardStateAndDispatch(first, CardState.Hidden);
            SetCardStateAndDispatch(second, CardState.Hidden);
            state.FlippedCards.Clear();
            events.Publish(new CardsMismatched(first, second));
        }

        protected virtual void SetCardStateAndDispatch(Card card, CardState newState)
        {
            card.State = newState;
            events.Publish(new CardStateChanged(card, newState));
        }

        private void UpdateCompletedFromState(GameState state)
        {
            isCompleted = AllCardsScored(state);
        }

        private void UpdateCompletedState(GameState state)
        {
            if (AllCardsScored(state))
            {
                isCompleted = true;
            }
        }

        private static bool AllCardsScored(GameState state)
        {
            if (state.Cards == null || state.Cards.Count == 0)
            {
                return false;
            }
            foreach (KeyValuePair<int, Card> pair in state.Cards)
            {
                if (pair.Value.State != CardState.Scored)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
