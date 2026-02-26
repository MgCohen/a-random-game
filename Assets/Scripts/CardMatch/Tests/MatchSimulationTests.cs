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
            bool completedReceived = false;
            ScoreRules rules = new ScoreRules();
            var match = new Match(state, rules);
            match.Subscribe<MatchCompleted>(e => { completedReceived = true; });
            SimulatePerfectPlay(match, state);
            Assert.That(completedReceived, Is.True);
            AssertAllCardsScored(state);
        }

        [Test]
        [TestCase(2, 2)]
        [TestCase(2, 4)]
        [TestCase(4, 4)]
        public void SimulateMatch_VariousBoardSizes_AllEndWithMatchCompleted(int rows, int cols)
        {
            GameState state = CreateShuffledBoard(rows, cols);
            int completedCount = 0;
            ScoreRules rules = new ScoreRules();
            var match = new Match(state, rules);
            match.Subscribe<MatchCompleted>(_ => completedCount += 1);
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
            for (int i = 0; i < state.Cards.Count; i++)
            {
                int id = state.Cards[i].CardId;
                if (!indicesByCardId.TryGetValue(id, out var list))
                {
                    list = new List<int>();
                    indicesByCardId[id] = list;
                }
                list.Add(i);
            }
            return indicesByCardId;
        }

        private static void AssertAllCardsScored(GameState state)
        {
            foreach (Card card in state.Cards)
            {
                Assert.That(card.State, Is.EqualTo(CardState.Scored));
            }
        }

        private static GameState CreateShuffledBoard(int rows, int cols)
        {
            int slotCount = rows * cols;
            Assert.That(slotCount % 2, Is.EqualTo(0));
            var cardIds = BuildPairCardIds(slotCount / 2);
            Shuffle(cardIds, 12345);
            var cards = BuildCardsList(cardIds);
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

        private static List<Card> BuildCardsList(IList<int> cardIds)
        {
            var cards = new List<Card>(cardIds.Count);
            for (int i = 0; i < cardIds.Count; i++)
            {
                cards.Add(new Card(cardIds[i], CardState.Hidden));
            }
            return cards;
        }

        private static GameState NewGameState(int rows, int cols, List<Card> cards)
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
