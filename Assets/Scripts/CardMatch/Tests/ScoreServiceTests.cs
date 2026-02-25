using System.Collections.Generic;
using CardMatch.CardMatch;
using NUnit.Framework;

namespace CardMatch.Tests
{
    public class ScoreServiceTests
    {
        [Test]
        public void MatchEvents_UpdateScoreAndCombo_AndEmitExpectedEvents()
        {
            var state = new GameState { Score = 0, Combo = 0 };
            var rules = new ScoreRules { BaseMatchPoints = 10, ComboBonusPerLevel = 5 };
            var events = new TypedEventService();
            var emitted = new List<string>();
            events.Subscribe<ScoreChanged>(e => emitted.Add($"score:{e.Score}:{e.AddedPoints}"));
            events.Subscribe<ComboChanged>(e => emitted.Add($"combo:{e.Combo}"));
            var service = new ScoreService(state, rules, events);
            var a1 = new Card(1);
            var a2 = new Card(1);
            var b1 = new Card(2);
            var b2 = new Card(2);
            events.Publish(new CardsMatched(a1, a2));
            events.Publish(new CardsMatched(b1, b2));

            Assert.That(state.Score, Is.EqualTo(25));
            Assert.That(state.Combo, Is.EqualTo(2));
            Assert.That(emitted, Is.EqualTo(new[]
            {
                "score:10:10",
                "combo:1",
                "score:25:15",
                "combo:2"
            }));
            service.Dispose();
        }

        [Test]
        public void MismatchEvent_ResetsCombo_AndDoesNotEmitWhenAlreadyZero()
        {
            var state = new GameState { Combo = 2 };
            var rules = new ScoreRules { BaseMatchPoints = 10, ComboBonusPerLevel = 5 };
            var events = new TypedEventService();
            var emittedCombos = new List<int>();
            events.Subscribe<ComboChanged>(e => emittedCombos.Add(e.Combo));
            var service = new ScoreService(state, rules, events);
            var c1 = new Card(1);
            var c2 = new Card(2);
            events.Publish(new CardsMismatched(c1, c2));
            events.Publish(new CardsMismatched(new Card(3), new Card(4)));

            Assert.That(state.Combo, Is.EqualTo(0));
            Assert.That(emittedCombos, Is.EqualTo(new[] { 0 }));
            service.Dispose();
        }

        [Test]
        public void Dispose_UnsubscribesFromEvents()
        {
            var state = new GameState { Score = 0, Combo = 0 };
            var rules = new ScoreRules { BaseMatchPoints = 10, ComboBonusPerLevel = 5 };
            var events = new TypedEventService();
            var service = new ScoreService(state, rules, events);
            service.Dispose();

            events.Publish(new CardsMatched(new Card(1), new Card(1)));
            events.Publish(new CardsMismatched(new Card(1), new Card(2)));

            Assert.That(state.Score, Is.EqualTo(0));
            Assert.That(state.Combo, Is.EqualTo(0));
        }
    }
}
