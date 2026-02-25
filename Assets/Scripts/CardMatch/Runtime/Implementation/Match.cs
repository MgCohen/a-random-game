using System.Collections.Generic;
using CardMatch.Config;

namespace CardMatch.CardMatch
{
    public class Match
    {
        private readonly GameState state;
        private readonly IMatchEvents events;
        private readonly IMatchEvaluator evaluator;
        private bool isGameOver;

        public GameState CurrentState => state;

        public Match(GameState initialState, IMatchEvents matchEvents)
        {
            state = initialState;
            events = matchEvents;
            evaluator = new MatchEvaluator(matchEvents);
            isGameOver = false;
        }

        public Match(GameSave save, IMatchEvents matchEvents)
        {
            state = SaveLoadMapping.ToState(save);
            events = matchEvents;
            evaluator = new MatchEvaluator(matchEvents);
            isGameOver = false;
            EvaluateState();
        }

        public void FlipCard(Card card)
        {
            if (!CanAcceptFlip(card))
            {
                return;
            }
            ApplyFlip(card);
        }

        private bool CanAcceptFlip(Card card)
        {
            if (isGameOver)
            {
                return false;
            }

            if(card == null)
            {
                return false;
            }

            if (!state.Cards.ContainsValue(card))
            {
                return false;
            }

            if(card.State != CardState.Hidden)
            {
                return false;
            }

            return true;
        }

        private void ApplyFlip(Card card)
        {
            state.FlippedCards.Add(card);
            ChangeCardState(card);
            EvaluateState();
        }

        private void ChangeCardState(Card card)
        {
            card.State = CardState.Flipped;
            events.Publish(new CardStateChanged(card, CardState.Flipped));
        }

        private void EvaluateState()
        {
            evaluator.Evaluate(state);
            EndGame();
        }

        private void EndGame()
        {
            if (!evaluator.IsCompleted)
            {
                return;
            }
            isGameOver = true;
            events.Publish(new MatchCompleted(state));
        }

    }
}
