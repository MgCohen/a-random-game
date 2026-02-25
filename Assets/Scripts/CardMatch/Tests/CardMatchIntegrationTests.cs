using System.Collections.Generic;
using CardMatch.CardMatch;
using CardMatch.Config;
using NUnit.Framework;

namespace CardMatch.Tests
{
    public class CardMatchIntegrationTests
    {
        [Test]
        public void Match_MatchedPair_EmitsExpectedEventOrder()
        {
            GameState state = CreateGameState(new[] { 1, 1 });
            var events = new TypedEventService();
            var sequence = new List<string>();
            events.Subscribe<CardStateChanged>(changed => sequence.Add($"card:{changed.Card.CardId}:{changed.State}"));
            events.Subscribe<CardsMatched>(matched => sequence.Add($"matched:{matched.First.CardId}:{matched.Second.CardId}"));
            events.Subscribe<MatchCompleted>(_ => sequence.Add("completed"));
            var match = new Match(state, events);
            FlipPair(match, state.Cards[0], state.Cards[1]);
            var expected = new[] { "card:1:Flipped", "card:1:Flipped", "card:1:Scored", "card:1:Scored", "matched:1:1", "completed" };
            CollectionAssert.AreEqual(expected, sequence);
        }

        [Test]
        public void ScoreService_ReactsToMatchEvents_AndEmitsScoreEvents()
        {
            GameState state = CreateGameState(new[] { 2, 2 });
            var rules = new ScoreRules { BaseMatchPoints = 10, ComboBonusPerLevel = 5 };
            var events = new TypedEventService();
            var scoreEvents = new List<ScoreChanged>();
            var comboEvents = new List<ComboChanged>();
            events.Subscribe<ScoreChanged>(changed => scoreEvents.Add(changed));
            events.Subscribe<ComboChanged>(changed => comboEvents.Add(changed));
            var scoreService = new ScoreService(state, rules, events);
            events.Publish(new CardsMatched(state.Cards[0], state.Cards[1]));
            events.Publish(new CardsMismatched(new Card(2), new Card(1)));
            Assert.That(state.Score, Is.EqualTo(10));
            Assert.That(state.Combo, Is.EqualTo(0));
            Assert.That(scoreEvents.Count, Is.EqualTo(1));
            Assert.That(scoreEvents[0].AddedPoints, Is.EqualTo(10));
            Assert.That(comboEvents.Count, Is.EqualTo(2));
            Assert.That(comboEvents[0].Combo, Is.EqualTo(1));
            Assert.That(comboEvents[1].Combo, Is.EqualTo(0));
            scoreService.Dispose();
        }

        private static void FlipPair(Match match, Card first, Card second)
        {
            match.FlipCard(first);
            match.FlipCard(second);
        }

        private static GameState CreateGameState(int[] ids)
        {
            var cards = new Dictionary<int, Card>();
            for (int i = 0; i < ids.Length; i++)
            {
                cards[i] = new Card(ids[i], CardState.Hidden);
            }
            return new GameState
            {
                Layout = new LayoutConfig { Rows = 1, Columns = ids.Length },
                Cards = cards,
                FlippedCards = new List<Card>(),
                Score = 0,
                Round = 0,
                Combo = 0
            };
        }
    }
}
