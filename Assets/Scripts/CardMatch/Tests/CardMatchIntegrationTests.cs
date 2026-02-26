using System.Collections.Generic;
using CardMatch.CardMatch;
using NUnit.Framework;

namespace CardMatch.Tests
{
    public class CardMatchIntegrationTests
    {
        [Test]
        public void Match_MatchedPair_EmitsExpectedEventOrder()
        {
            GameState state = CreateGameState(new[] { 1, 1 });
            var sequence = new List<string>();
            ScoreRules rules = new ScoreRules();
            var match = new Match(state, rules);
            match.Subscribe<CardStateChanged>(changed => sequence.Add($"card:{changed.Card.CardId}:{changed.State}"));
            match.Subscribe<CardsMatched>(matched => sequence.Add($"matched:{matched.First.CardId}:{matched.Second.CardId}"));
            match.Subscribe<MatchCompleted>(_ => sequence.Add("completed"));
            FlipPair(match, state.Cards[0], state.Cards[1]);
            var expected = new[] { "card:1:Flipped", "card:1:Flipped", "card:1:Scored", "card:1:Scored", "matched:1:1", "completed" };
            CollectionAssert.AreEqual(expected, sequence);
        }

        [Test]
        public void Match_InternalScoreService_EmitsScoreAndComboEvents()
        {
            GameState state = CreateGameState(new[] { 2, 2 });
            var rules = new ScoreRules { BaseMatchPoints = 10, ComboBonusPerLevel = 5 };
            var scoreEvents = new List<ScoreChanged>();
            var comboEvents = new List<ComboChanged>();
            var match = new Match(state, rules);
            match.Subscribe<ScoreChanged>(changed => scoreEvents.Add(changed));
            match.Subscribe<ComboChanged>(changed => comboEvents.Add(changed));
            match.FlipCard(state.Cards[0]);
            match.FlipCard(state.Cards[1]);
            Assert.That(state.Score, Is.EqualTo(10));
            Assert.That(state.Combo, Is.EqualTo(1));
            Assert.That(scoreEvents.Count, Is.EqualTo(1));
            Assert.That(scoreEvents[0].AddedPoints, Is.EqualTo(10));
            Assert.That(comboEvents.Count, Is.EqualTo(1));
            Assert.That(comboEvents[0].Combo, Is.EqualTo(1));
        }

        private static void FlipPair(Match match, Card first, Card second)
        {
            match.FlipCard(first);
            match.FlipCard(second);
        }

        private static GameState CreateGameState(int[] ids)
        {
            var cards = new List<Card>(ids.Length);
            for (int i = 0; i < ids.Length; i++)
            {
                cards.Add(new Card(ids[i], CardState.Hidden));
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
