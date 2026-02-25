using System.Collections.Generic;
using CardMatch.CardMatch;
using NUnit.Framework;

namespace CardMatch.Tests
{
    public class MatchEvaluatorTests
    {
        [Test]
        public void Evaluate_WhenFlippedCardsNull_DoesNothing()
        {
            var state = new GameState { FlippedCards = null };
            var events = new TypedEventService();
            int published = 0;
            events.Subscribe<CardStateChanged>(_ => published += 1);
            events.Subscribe<CardsMatched>(_ => published += 1);
            events.Subscribe<CardsMismatched>(_ => published += 1);
            var evaluator = new MatchEvaluator(events);
            evaluator.Evaluate(state);
            Assert.That(published, Is.EqualTo(0));
        }

        [Test]
        public void Evaluate_WhenFlippedCardsEmpty_DoesNothing()
        {
            var state = new GameState();
            state.FlippedCards = new List<Card>();
            var events = new TypedEventService();
            int published = 0;
            events.Subscribe<CardStateChanged>(_ => published += 1);
            var evaluator = new MatchEvaluator(events);
            evaluator.Evaluate(state);
            Assert.That(published, Is.EqualTo(0));
        }

        [Test]
        public void Evaluate_WhenOneFlippedCard_DoesNothing()
        {
            Card first = new Card(1, CardState.Flipped);
            var state = CreateStateWithFlipped(first);
            var events = new TypedEventService();
            int published = 0;
            events.Subscribe<CardStateChanged>(_ => published += 1);
            var evaluator = new MatchEvaluator(events);
            evaluator.Evaluate(state);
            Assert.That(published, Is.EqualTo(0));
            Assert.That(state.FlippedCards.Count, Is.EqualTo(1));
        }

        [Test]
        public void Evaluate_WhenTwoFlippedMatchingCards_AppliesMatchAndPublishesEvents()
        {
            Card first = new Card(7, CardState.Flipped);
            Card second = new Card(7, CardState.Flipped);
            var state = CreateStateWithFlipped(first, second);
            state.Cards[0] = first;
            state.Cards[1] = second;
            var events = new TypedEventService();
            var stateChangedEvents = new List<CardStateChanged>();
            CardsMatched matchedEvent = null;
            events.Subscribe<CardStateChanged>(e => stateChangedEvents.Add(e));
            events.Subscribe<CardsMatched>(e => matchedEvent = e);
            var evaluator = new MatchEvaluator(events);
            evaluator.Evaluate(state);
            Assert.That(first.State, Is.EqualTo(CardState.Scored));
            Assert.That(second.State, Is.EqualTo(CardState.Scored));
            Assert.That(state.FlippedCards.Count, Is.EqualTo(0));
            Assert.That(state.Round, Is.EqualTo(1));
            Assert.That(stateChangedEvents.Count, Is.EqualTo(2));
            Assert.That(stateChangedEvents[0].Card, Is.SameAs(first));
            Assert.That(stateChangedEvents[0].State, Is.EqualTo(CardState.Scored));
            Assert.That(stateChangedEvents[1].Card, Is.SameAs(second));
            Assert.That(stateChangedEvents[1].State, Is.EqualTo(CardState.Scored));
            Assert.That(matchedEvent, Is.Not.Null);
            Assert.That(matchedEvent.First, Is.SameAs(first));
            Assert.That(matchedEvent.Second, Is.SameAs(second));
        }

        [Test]
        public void Evaluate_WhenTwoFlippedMatchingCards_IsCompletedTrueWhenAllScored()
        {
            Card first = new Card(5, CardState.Flipped);
            Card second = new Card(5, CardState.Flipped);
            var state = CreateStateWithFlipped(first, second);
            state.Cards[0] = first;
            state.Cards[1] = second;
            var events = new TypedEventService();
            var evaluator = new MatchEvaluator(events);
            evaluator.Evaluate(state);
            Assert.That(evaluator.IsCompleted, Is.True);
        }

        [Test]
        public void Evaluate_WhenTwoFlippedMatchingCards_IsCompletedFalseWhenMoreCardsRemain()
        {
            Card first = new Card(1, CardState.Flipped);
            Card second = new Card(1, CardState.Flipped);
            Card third = new Card(2, CardState.Hidden);
            Card fourth = new Card(2, CardState.Hidden);
            var state = CreateStateWithFlipped(first, second);
            state.Cards[0] = first;
            state.Cards[1] = second;
            state.Cards[2] = third;
            state.Cards[3] = fourth;
            var events = new TypedEventService();
            var evaluator = new MatchEvaluator(events);
            evaluator.Evaluate(state);
            Assert.That(evaluator.IsCompleted, Is.False);
        }

        [Test]
        public void Evaluate_WhenTwoFlippedMismatchingCards_AppliesMismatchAndPublishesEvents()
        {
            Card first = new Card(1, CardState.Flipped);
            Card second = new Card(2, CardState.Flipped);
            var state = CreateStateWithFlipped(first, second);
            var events = new TypedEventService();
            var stateChangedEvents = new List<CardStateChanged>();
            CardsMismatched mismatchedEvent = null;
            events.Subscribe<CardStateChanged>(e => stateChangedEvents.Add(e));
            events.Subscribe<CardsMismatched>(e => mismatchedEvent = e);
            var evaluator = new MatchEvaluator(events);
            evaluator.Evaluate(state);
            Assert.That(first.State, Is.EqualTo(CardState.Hidden));
            Assert.That(second.State, Is.EqualTo(CardState.Hidden));
            Assert.That(state.FlippedCards.Count, Is.EqualTo(0));
            Assert.That(stateChangedEvents.Count, Is.EqualTo(2));
            Assert.That(stateChangedEvents[0].Card, Is.SameAs(first));
            Assert.That(stateChangedEvents[0].State, Is.EqualTo(CardState.Hidden));
            Assert.That(stateChangedEvents[1].Card, Is.SameAs(second));
            Assert.That(stateChangedEvents[1].State, Is.EqualTo(CardState.Hidden));
            Assert.That(mismatchedEvent, Is.Not.Null);
            Assert.That(mismatchedEvent.First, Is.SameAs(first));
            Assert.That(mismatchedEvent.Second, Is.SameAs(second));
        }

        [Test]
        public void Evaluate_UpdatesIsCompletedFromStateAtStart()
        {
            Card first = new Card(1, CardState.Scored);
            Card second = new Card(1, CardState.Scored);
            var state = new GameState();
            state.Cards = new Dictionary<int, Card> { [0] = first, [1] = second };
            state.FlippedCards = new List<Card>();
            var events = new TypedEventService();
            var evaluator = new MatchEvaluator(events);
            evaluator.Evaluate(state);
            Assert.That(evaluator.IsCompleted, Is.True);
        }

        private static GameState CreateStateWithFlipped(Card first, Card second = null)
        {
            var flipped = new List<Card> { first };
            if (second != null)
            {
                flipped.Add(second);
            }
            var state = new GameState();
            state.Cards = new Dictionary<int, Card>();
            state.FlippedCards = flipped;
            state.Round = 0;
            return state;
        }
    }
}
