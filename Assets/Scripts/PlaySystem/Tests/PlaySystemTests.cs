using CardMatch.CardMatch;
using CardMatch.Levels;
using CardMatch.Navigation;
using CardMatch.PlaySystem;
using NUnit.Framework;
using UnityEngine;

namespace CardMatch.PlaySystem.Tests
{
    public class PlaySystemTests
    {
        [Test]
        public void Play_OpensPlayViewContextWithMatchAndLevel()
        {
            Level level = ScriptableObject.CreateInstance<Level>();
            var nav = new FakeNavigation();
            var match = new Match(new GameState(), new ScoreRules());
            var factory = new FakePlayMatchFactory { MatchToReturn = match };
            var levelController = new FakeLevelController();
            var playSystem = new PlaySystem(nav, factory, levelController);
            playSystem.Play(level);
            PlayViewContext ctx = nav.OpenedContext as PlayViewContext;
            Assert.That(ctx, Is.Not.Null);
            Assert.That(ctx.Match, Is.SameAs(match));
            Assert.That(ctx.Level, Is.SameAs(level));
            Assert.That(ctx.PlaySystem, Is.SameAs(playSystem));
            Object.DestroyImmediate(level);
        }

        [Test]
        public void Play_WithNullLevel_DoesNotOpenView()
        {
            var nav = new FakeNavigation();
            var playSystem = new PlaySystem(nav, null, null);
            playSystem.Play(null);
            Assert.That(nav.OpenedContext, Is.Null);
        }

        [Test]
        public void Play_WithNullNavigation_DoesNotThrow()
        {
            Level level = ScriptableObject.CreateInstance<Level>();
            var playSystem = new PlaySystem(null, null, null);
            playSystem.Play(level);
            Object.DestroyImmediate(level);
        }

        [Test]
        public void GoBack_CallsNavigationGoBack()
        {
            var nav = new FakeNavigation();
            var playSystem = new PlaySystem(nav, null, null);
            playSystem.GoBack();
            Assert.That(nav.GoBackCallCount, Is.EqualTo(1));
        }

        [Test]
        public void GoBack_WithNullNavigation_DoesNotThrow()
        {
            var playSystem = new PlaySystem(null, null, null);
            playSystem.GoBack();
        }

        [Test]
        public void MarkCompleted_CallsLevelControllerMarkCompleted()
        {
            Level level = ScriptableObject.CreateInstance<Level>();
            var nav = new FakeNavigation();
            var levelController = new FakeLevelController();
            var playSystem = new PlaySystem(nav, null, levelController);
            playSystem.MarkCompleted(level);
            Assert.That(levelController.MarkCompletedCalledWith, Is.SameAs(level));
            Object.DestroyImmediate(level);
        }

        [Test]
        public void MarkCompleted_WithNullLevel_DoesNotThrow()
        {
            var levelController = new FakeLevelController();
            var playSystem = new PlaySystem(null, null, levelController);
            playSystem.MarkCompleted(null);
            Assert.That(levelController.MarkCompletedCalledWith, Is.Null);
        }

        [Test]
        public void MarkCompleted_WithNullLevelController_DoesNotThrow()
        {
            Level level = ScriptableObject.CreateInstance<Level>();
            var playSystem = new PlaySystem(null, null, null);
            playSystem.MarkCompleted(level);
            Object.DestroyImmediate(level);
        }

        private sealed class FakeNavigation : INavigation
        {
            public int StackCount => 0;
            public IView CurrentView => null;
            public IViewContext OpenedContext { get; private set; }
            public int GoBackCallCount { get; private set; }
            public void Open<T>(T context, bool closeCurrent = false) where T : IViewContext { OpenedContext = context; }
            public void GoBack() { GoBackCallCount += 1; }
            public void Focus(IView view) { }
        }

        private sealed class FakePlayMatchFactory : IPlayMatchFactory
        {
            public Match MatchToReturn { get; set; }
            public Match Create(Level level) => MatchToReturn;
        }

        private sealed class FakeLevelController : ILevelController
        {
            public Level MarkCompletedCalledWith { get; private set; }
            public System.Collections.Generic.IReadOnlyList<Level> GetLevels() => new Level[0];
            public Level GetLevel(int index) => null;
            public bool IsUnlocked(Level level) => false;
            public bool IsCompleted(Level level) => false;
            public void MarkCompleted(Level level) { MarkCompletedCalledWith = level; }
        }
    }
}
