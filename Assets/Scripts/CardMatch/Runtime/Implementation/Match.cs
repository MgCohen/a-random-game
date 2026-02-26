using System;
using System.Collections.Generic;
using CardMatch.Config;

namespace CardMatch.CardMatch
{
    public class Match : IDisposable
    {
        private readonly GameState state;
        private readonly IMatchEvents events;
        private readonly IScoreService scoreService;
        private readonly IMatchEvaluator evaluator;
        private bool isGameOver;

        public GameState CurrentState => state;

        public Match(GameState initialState, ScoreRules scoreRules)
        {
            state = initialState;
            events = new TypedEventService();
            evaluator = new MatchEvaluator(events);
            scoreService = new ScoreService(state, scoreRules, events);
            isGameOver = false;
        }

        public Match(GameSave save, ScoreRules scoreRules)
        {
            state = SaveLoadMapping.ToState(save);
            events = new TypedEventService();
            evaluator = new MatchEvaluator(events);
            scoreService = new ScoreService(state, scoreRules, events);
            isGameOver = false;
            EvaluateState();
        }

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : MatchEvent
        {
            events.Subscribe(handler);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : MatchEvent
        {
            events.Unsubscribe(handler);
        }

        public void FlipCard(Card card)
        {
            bool canAcceptFlip = CanAcceptFlip(card);
            if (canAcceptFlip) ApplyFlip(card);
        }

        private bool CanAcceptFlip(Card card)
        {
            if (!IsGameRunning())
            {
                return false;
            }

            if (!IsCardReferenceValid(card))
            {
                return false;
            }

            if (!IsCardHidden(card))
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

        private bool IsGameRunning()
        {
            return !isGameOver;
        }

        private bool IsCardReferenceValid(Card card)
        {
            return card != null;
        }

        private bool IsCardHidden(Card card)
        {
            if (card == null) return false;
            return card.State == CardState.Hidden;
        }

        private void ChangeCardState(Card card)
        {
            card.State = CardState.Flipped;
            events.Publish(new CardStateChanged(card, CardState.Flipped));
        }

        private void EvaluateState()
        {
            evaluator.Evaluate(state);
            VerifyEndGame();
        }

        private void VerifyEndGame()
        {
            if (evaluator.IsCompleted)
            {
                CompleteGame();
            }
        }

        private void CompleteGame()
        {
            isGameOver = true;
            events.Publish(new MatchCompleted(true));
        }

        public void Dispose()
        {
            scoreService?.Dispose();
        }
    }
}
