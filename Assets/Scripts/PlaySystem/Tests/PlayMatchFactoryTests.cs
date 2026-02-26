using System.Collections.Generic;
using System.Reflection;
using CardMatch.CardMatch;
using CardMatch.PlaySystem;
using NUnit.Framework;
using UnityEngine;

namespace CardMatch.PlaySystem.Tests
{
    public class PlayMatchFactoryTests
    {
        [Test]
        public void Create_WhenLevelIsNull_ReturnsNull()
        {
            var factory = new PlayMatchFactory();
            Match match = factory.Create(null);
            Assert.That(match, Is.Null);
        }

        [Test]
        public void Create_WhenLevelHasEvenSlotCount_BuildsMatchWithPairs()
        {
            Level level = CreateLevel("id0", 2, 4, 10, 1);
            var factory = new PlayMatchFactory();
            Match match = factory.Create(level);
            Assert.That(match, Is.Not.Null);
            GameState state = match.CurrentState;
            Assert.That(state, Is.Not.Null);
            Assert.That(state.Cards.Count, Is.EqualTo(8));
            Assert.That(state.Score, Is.EqualTo(0));
            Assert.That(state.Round, Is.EqualTo(0));
            Assert.That(state.Combo, Is.EqualTo(0));
            AssertAllCardsHaveExactlyOnePair(state.Cards);
            match.Dispose();
            Object.DestroyImmediate(level);
        }

        private static void AssertAllCardsHaveExactlyOnePair(IList<Card> cards)
        {
            var counts = new Dictionary<int, int>();
            foreach (Card card in cards)
            {
                int cardId = card.CardId;
                int count = 0;
                if (counts.TryGetValue(cardId, out int existing))
                {
                    count = existing;
                }
                counts[cardId] = count + 1;
            }
            foreach (KeyValuePair<int, int> pair in counts)
            {
                Assert.That(pair.Value, Is.EqualTo(2));
            }
        }

        private static Level CreateLevel(string levelId, int rows, int columns, int baseMatchPoints, int comboBonus)
        {
            Level level = ScriptableObject.CreateInstance<Level>();
            LevelConfig config = CreateConfig(rows, columns, baseMatchPoints, comboBonus);
            SetPrivateField(level, "levelId", levelId);
            SetPrivateField(level, "config", config);
            return level;
        }

        private static LevelConfig CreateConfig(int rows, int columns, int baseMatchPoints, int comboBonus)
        {
            var config = new LevelConfig();
            var layout = new LayoutConfig();
            layout.Rows = rows;
            layout.Columns = columns;
            var scoring = new ScoreRules();
            scoring.BaseMatchPoints = baseMatchPoints;
            scoring.ComboBonusPerLevel = comboBonus;
            config.Layout = layout;
            config.Scoring = scoring;
            return config;
        }

        private static void SetPrivateField(object target, string name, object value)
        {
            FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }
    }
}
