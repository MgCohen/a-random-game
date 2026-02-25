using System.Collections.Generic;
using CardMatch.CardMatch;
using NUnit.Framework;

namespace CardMatch.Tests
{
    public class MatchTests
    {
        [Test]
        public void Constructor_UsesInjectedCardStateWithoutRebuilding()
        {
            Card first = new Card(11);
            Card second = new Card(12);
            var state = new GameState { Cards = new Dictionary<int, Card> { [5] = first, [2] = second } };
            var events = new TypedEventService();
            var match = new Match(state, events);
            Assert.That(match.CurrentState.Cards[5], Is.SameAs(first));
            Assert.That(match.CurrentState.Cards[2], Is.SameAs(second));
            Assert.That(match.CurrentState.Cards.Count, Is.EqualTo(2));
        }

        [Test]
        public void FlipCard_WhenStateHasNoCards_DoesNothing()
        {
            var state = new GameState();
            var events = new TypedEventService();
            int cardStateChangedCount = 0;
            int completedCount = 0;
            events.Subscribe<CardStateChanged>(_ => cardStateChangedCount += 1);
            events.Subscribe<MatchCompleted>(_ => completedCount += 1);
            var match = new Match(state, events);
            match.FlipCard(new Card(0));
            Assert.That(state.Cards.Count, Is.EqualTo(0));
            Assert.That(state.FlippedCards.Count, Is.EqualTo(0));
            Assert.That(cardStateChangedCount, Is.EqualTo(0));
            Assert.That(completedCount, Is.EqualTo(0));
        }

        [Test]
        public void FlipCard_WhenAllCardsAlreadyScored_IgnoresFlipAndDoesNotEmitCompleted()
        {
            GameState state = CreateState(new Card(2, CardState.Scored), new Card(2, CardState.Scored));
            var events = new TypedEventService();
            int cardStateChangedCount = 0;
            int completedCount = 0;
            events.Subscribe<CardStateChanged>(_ => cardStateChangedCount += 1);
            events.Subscribe<MatchCompleted>(_ => completedCount += 1);
            var match = new Match(state, events);
            match.FlipCard(state.Cards[0]);
            Assert.That(state.Cards[0].State, Is.EqualTo(CardState.Scored));
            Assert.That(state.Cards[1].State, Is.EqualTo(CardState.Scored));
            Assert.That(state.FlippedCards.Count, Is.EqualTo(0));
            Assert.That(cardStateChangedCount, Is.EqualTo(0));
            Assert.That(completedCount, Is.EqualTo(0));
        }

        [Test]
        public void FlipCard_WhenCardNotInState_DoesNothing()
        {
            GameState state = CreateState(new Card(1), new Card(1));
            var events = new TypedEventService();
            int cardStateChangedCount = 0;
            events.Subscribe<CardStateChanged>(_ => cardStateChangedCount += 1);
            var match = new Match(state, events);

            match.FlipCard(new Card(99));

            Assert.That(cardStateChangedCount, Is.EqualTo(0));
            Assert.That(state.FlippedCards.Count, Is.EqualTo(0));
            Assert.That(state.Cards[0].State, Is.EqualTo(CardState.Hidden));
            Assert.That(state.Cards[1].State, Is.EqualTo(CardState.Hidden));
        }

        [Test]
        public void FlipCard_WhenCardAlreadyFlipped_DoesNotFlipAgain()
        {
            GameState state = CreateState(new Card(1), new Card(2));
            var events = new TypedEventService();
            int cardStateChangedCount = 0;
            events.Subscribe<CardStateChanged>(_ => cardStateChangedCount += 1);
            var match = new Match(state, events);
            match.FlipCard(state.Cards[0]);
            match.FlipCard(state.Cards[0]);
            Assert.That(cardStateChangedCount, Is.EqualTo(1));
            Assert.That(state.FlippedCards.Count, Is.EqualTo(1));
            Assert.That(state.Cards[0].State, Is.EqualTo(CardState.Flipped));
        }

        [Test]
        public void FlipCard_WhenPairMatches_EmitsExpectedEventsInOrder()
        {
            GameState state = CreateState(new Card(7), new Card(7), new Card(9), new Card(9));
            var events = new TypedEventService();
            var emitted = new List<string>();
            events.Subscribe<CardStateChanged>(e => emitted.Add($"state:{e.Card.CardId}:{e.State}"));
            events.Subscribe<CardsMatched>(e => emitted.Add($"matched:{e.First.CardId}:{e.Second.CardId}"));
            events.Subscribe<MatchCompleted>(_ => emitted.Add("completed"));
            var match = new Match(state, events);
            FlipPair(match, state.Cards[0], state.Cards[1]);
            Assert.That(state.Cards[0].State, Is.EqualTo(CardState.Scored));
            Assert.That(state.Cards[1].State, Is.EqualTo(CardState.Scored));
            Assert.That(state.Round, Is.EqualTo(1));
            Assert.That(state.FlippedCards.Count, Is.EqualTo(0));
            Assert.That(emitted, Is.EqualTo(new[] { "state:7:Flipped", "state:7:Flipped", "state:7:Scored", "state:7:Scored", "matched:7:7" }));
        }

        [Test]
        public void FlipCard_WhenPairMismatches_ResetsCardsAndEmitsMismatch()
        {
            GameState state = CreateState(new Card(1), new Card(2));
            var events = new TypedEventService();
            var emitted = new List<string>();
            events.Subscribe<CardStateChanged>(e => emitted.Add($"state:{e.Card.CardId}:{e.State}"));
            events.Subscribe<CardsMismatched>(e => emitted.Add($"mismatched:{e.First.CardId}:{e.Second.CardId}"));
            var match = new Match(state, events);
            FlipPair(match, state.Cards[0], state.Cards[1]);
            Assert.That(state.Cards[0].State, Is.EqualTo(CardState.Hidden));
            Assert.That(state.Cards[1].State, Is.EqualTo(CardState.Hidden));
            Assert.That(state.Round, Is.EqualTo(0));
            Assert.That(state.FlippedCards.Count, Is.EqualTo(0));
            Assert.That(emitted, Is.EqualTo(new[] { "state:1:Flipped", "state:2:Flipped", "state:1:Hidden", "state:2:Hidden", "mismatched:1:2" }));
        }

        [Test]
        public void FlipCard_WhenFinalPairMatches_EmitsCompletedOnlyOnce()
        {
            GameState state = CreateState(new Card(4), new Card(4));
            var events = new TypedEventService();
            int completedCount = 0;
            events.Subscribe<MatchCompleted>(_ => completedCount += 1);
            var match = new Match(state, events);
            FlipPair(match, state.Cards[0], state.Cards[1]);
            FlipPair(match, state.Cards[0], state.Cards[1]);
            Assert.That(completedCount, Is.EqualTo(1));
        }

        [Test]
        public void FlipCard_MatchDoesNotChangeScoreOrComboWithoutScoreService()
        {
            GameState state = CreateState(new Card(5), new Card(5));
            state.Score = 100;
            state.Combo = 3;
            var events = new TypedEventService();
            int cardStateChangedCount = 0;
            int matchedCount = 0;
            MatchCompleted completedEvent = null;
            events.Subscribe<CardStateChanged>(_ => cardStateChangedCount += 1);
            events.Subscribe<CardsMatched>(_ => matchedCount += 1);
            events.Subscribe<MatchCompleted>(e => completedEvent = e);
            var match = new Match(state, events);
            FlipPair(match, state.Cards[0], state.Cards[1]);
            Assert.That(state.Score, Is.EqualTo(100));
            Assert.That(state.Combo, Is.EqualTo(3));
            Assert.That(cardStateChangedCount, Is.EqualTo(4));
            Assert.That(matchedCount, Is.EqualTo(1));
            Assert.That(completedEvent, Is.Not.Null);
            Assert.That(completedEvent.FinalState, Is.SameAs(state));
        }

        private static void FlipPair(Match match, Card first, Card second)
        {
            match.FlipCard(first);
            match.FlipCard(second);
        }

        private static GameState CreateState(params Card[] cards)
        {
            var gameState = new GameState();
            for (int i = 0; i < cards.Length; i++)
            {
                gameState.Cards[i] = cards[i];
            }
            return gameState;
        }
    }
}
