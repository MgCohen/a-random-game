using CardMatch.CardMatch;
using CardMatch.PlaySystem;
using NUnit.Framework;
using UnityEngine;

namespace CardMatch.PlaySystem.Tests
{
    public class PlayViewContextTests
    {
        [Test]
        public void Constructor_SetsAllDependencies()
        {
            Level level = ScriptableObject.CreateInstance<Level>();
            var match = new Match(new GameState(), new ScoreRules());
            IPlaySystem playSystem = new FakePlaySystem();
            var context = new PlayViewContext(match, level, playSystem);
            Assert.That(context.Match, Is.SameAs(match));
            Assert.That(context.Level, Is.SameAs(level));
            Assert.That(context.PlaySystem, Is.SameAs(playSystem));
            match.Dispose();
            Object.DestroyImmediate(level);
        }

        private sealed class FakePlaySystem : IPlaySystem
        {
            public void Play(Level level) { }
            public void GoBack() { }
            public void MarkCompleted(Level level) { }
        }
    }
}
