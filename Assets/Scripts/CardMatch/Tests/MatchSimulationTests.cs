using System.Collections.Generic;
using CardMatch.CardMatch;
using NUnit.Framework;

namespace CardMatch.Tests
{
    public class MatchSimulationTests
    {
        [Test]
        public void SimulateMatch_4x4Board_EndsWithMatchCompleted()
        {
            int rows = 4;
            int cols = 4;
            GameState state = CreateShuffledBoard(rows, cols);
            var events = new TypedEventService();
            bool completedReceived = false;
            GameState finalState = null;
            events.Subscribe<MatchCompleted>(e => { completedReceived = true; finalState = e.FinalState; });
            var match = new Match(state, events);
            SimulatePerfectPlay(match, state);
            Assert.That(completedReceived, Is.True);
            Assert.That(finalState, Is.Not.Null);
            Assert.That(finalState.Cards.Count, Is.EqualTo(rows * cols));
            AssertAllCardsScored(finalState);
            Assert.That(finalState.Round, Is.EqualTo(rows * cols / 2));
        }

        [Test]
        [TestCase(2, 2)]
        [TestCase(2, 4)]
        [TestCase(4, 4)]
        public void SimulateMatch_VariousBoardSizes_AllEndWithMatchCompleted(int rows, int cols)
        {
            GameState state = CreateShuffledBoard(rows, cols);
            var events = new TypedEventService();
            int completedCount = 0;
            events.Subscribe<MatchCompleted>(_ => completedCount += 1);
            var match = new Match(state, events);
            SimulatePerfectPlay(match, state);
            Assert.That(completedCount, Is.EqualTo(1));
        }

        private static void SimulatePerfectPlay(Match match, GameState state)
        {
            var indicesByCardId = BuildIndicesByCardId(state);
            foreach (var kv in indicesByCardId)
            {
                List<int> indices = kv.Value;
                Assert.That(indices.Count, Is.EqualTo(2));
                Card first = state.Cards[indices[0]];
                Card second = state.Cards[indices[1]];
                match.FlipCard(first);
                match.FlipCard(second);
            }
        }

        private static Dictionary<int, List<int>> BuildIndicesByCardId(GameState state)
        {
            var indicesByCardId = new Dictionary<int, List<int>>();
            foreach (var kv in state.Cards)
            {
                int id = kv.Value.CardId;
                if (!indicesByCardId.TryGetValue(id, out var list))
                {
                    list = new List<int>();
                    indicesByCardId[id] = list;
                }
                list.Add(kv.Key);
            }
            return indicesByCardId;
        }

        private static void AssertAllCardsScored(GameState state)
        {
            foreach (var kv in state.Cards)
            {
                Assert.That(kv.Value.State, Is.EqualTo(CardState.Scored));
            }
        }

        private static GameState CreateShuffledBoard(int rows, int cols)
        {
            int slotCount = rows * cols;
            Assert.That(slotCount % 2, Is.EqualTo(0));
            var cardIds = BuildPairCardIds(slotCount / 2);
            Shuffle(cardIds, 12345);
            var cards = BuildCardsDictionary(cardIds);
            return NewGameState(rows, cols, cards);
        }

        private static List<int> BuildPairCardIds(int pairCount)
        {
            var cardIds = new List<int>();
            for (int id = 1; id <= pairCount; id++)
            {
                cardIds.Add(id);
                cardIds.Add(id);
            }
            return cardIds;
        }

        private static Dictionary<int, Card> BuildCardsDictionary(IList<int> cardIds)
        {
            var cards = new Dictionary<int, Card>();
            for (int i = 0; i < cardIds.Count; i++)
            {
                cards[i] = new Card(cardIds[i], CardState.Hidden);
            }
            return cards;
        }

        private static GameState NewGameState(int rows, int cols, Dictionary<int, Card> cards)
        {
            return new GameState
            {
                Layout = new LayoutConfig { Rows = rows, Columns = cols },
                Cards = cards,
                FlippedCards = new List<Card>(),
                Score = 0,
                Round = 0,
                Combo = 0
            };
        }

        private static void Shuffle(IList<int> list, int seed)
        {
            var rng = new System.Random(seed);
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                int t = list[i];
                list[i] = list[j];
                list[j] = t;
            }
        }
    }
}
