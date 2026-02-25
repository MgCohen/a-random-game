using System.Collections.Generic;
using System.Reflection;
using CardMatch.CardMatch;
using CardMatch.Levels;
using NUnit.Framework;
using UnityEngine;

namespace CardMatch.Levels.Tests
{
    public class LevelsTests
    {
        [Test]
        public void GetLevels_WhenRegistryEmpty_ReturnsEmptyList()
        {
            LevelRegistry registry = CreateRegistry();
            var handler = new LevelProgressSaveHandler();
            LevelCompletionState state = handler.ToState(new LevelProgressSave(), new List<Level>());
            ILevelController controller = new LevelController(registry, state);
            Assert.That(controller.GetLevels().Count, Is.EqualTo(0));
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void GetLevels_WhenRegistryHasLevels_ReturnsRegistryOrder()
        {
            Level level0 = CreateLevel("id0");
            Level level1 = CreateLevel("id1");
            LevelRegistry registry = CreateRegistry(level0, level1);
            LevelCompletionState state = CreateState(level0, level1);
            ILevelController controller = new LevelController(registry, state);
            var list = controller.GetLevels();
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0], Is.SameAs(level0));
            Assert.That(list[1], Is.SameAs(level1));
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(level1);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void GetLevel_ValidIndex_ReturnsLevel()
        {
            Level level0 = CreateLevel("id0");
            LevelRegistry registry = CreateRegistry(level0);
            LevelCompletionState state = CreateState(level0);
            ILevelController controller = new LevelController(registry, state);
            Assert.That(controller.GetLevel(0), Is.SameAs(level0));
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void GetLevel_OutOfRange_ReturnsNull()
        {
            LevelRegistry registry = CreateRegistry();
            var handler = new LevelProgressSaveHandler();
            LevelCompletionState state = handler.ToState(new LevelProgressSave(), new List<Level>());
            ILevelController controller = new LevelController(registry, state);
            Assert.That(controller.GetLevel(-1), Is.Null);
            Assert.That(controller.GetLevel(0), Is.Null);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void IsUnlocked_FirstLevel_IsTrue()
        {
            Level level0 = CreateLevel("id0");
            LevelRegistry registry = CreateRegistry(level0);
            LevelCompletionState state = CreateState(level0);
            ILevelController controller = new LevelController(registry, state);
            Assert.That(controller.IsUnlocked(level0), Is.True);
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void IsUnlocked_SecondLevelBeforeFirstCompleted_IsFalse()
        {
            Level level0 = CreateLevel("id0");
            Level level1 = CreateLevel("id1");
            LevelRegistry registry = CreateRegistry(level0, level1);
            LevelCompletionState state = CreateState(level0, level1);
            ILevelController controller = new LevelController(registry, state);
            Assert.That(controller.IsUnlocked(level1), Is.False);
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(level1);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void MarkCompleted_ThenIsCompleted_IsTrue()
        {
            Level level0 = CreateLevel("id0");
            LevelRegistry registry = CreateRegistry(level0);
            LevelCompletionState state = CreateState(level0);
            ILevelController controller = new LevelController(registry, state);
            Assert.That(controller.IsCompleted(level0), Is.False);
            controller.MarkCompleted(level0);
            Assert.That(controller.IsCompleted(level0), Is.True);
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void MarkCompleted_FirstLevel_UnlocksSecondLevel()
        {
            Level level0 = CreateLevel("id0");
            Level level1 = CreateLevel("id1");
            LevelRegistry registry = CreateRegistry(level0, level1);
            LevelCompletionState state = CreateState(level0, level1);
            ILevelController controller = new LevelController(registry, state);
            Assert.That(controller.IsUnlocked(level1), Is.False);
            controller.MarkCompleted(level0);
            Assert.That(controller.IsUnlocked(level1), Is.True);
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(level1);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void ToPersistence_EmptyState_ReturnsEmptyEntries()
        {
            var state = new LevelCompletionState();
            var handler = new LevelProgressSaveHandler();
            LevelProgressSave save = handler.ToPersistence(state);
            Assert.That(save.Entries, Is.Not.Null);
            Assert.That(save.Entries.Length, Is.EqualTo(0));
        }

        [Test]
        public void ToPersistence_StateWithOneCompletedLevel_ReturnsEntryWithCompletedState()
        {
            var state = new LevelCompletionState();
            state.SetState("id0", LevelProgressState.Completed);
            var handler = new LevelProgressSaveHandler();
            LevelProgressSave save = handler.ToPersistence(state);
            Assert.That(save.Entries.Length, Is.EqualTo(1));
            Assert.That(save.Entries[0].LevelId, Is.EqualTo("id0"));
            Assert.That(save.Entries[0].State, Is.EqualTo((int)LevelProgressState.Completed));
        }

        [Test]
        public void ToPersistence_StateWithMultipleLevels_ReturnsAllEntriesWithCorrectStates()
        {
            var state = new LevelCompletionState();
            state.SetState("a", LevelProgressState.Locked);
            state.SetState("b", LevelProgressState.Unlocked);
            state.SetState("c", LevelProgressState.Completed);
            var handler = new LevelProgressSaveHandler();
            LevelProgressSave save = handler.ToPersistence(state);
            Assert.That(save.Entries.Length, Is.EqualTo(3));
            AssertEntry(save, "a", LevelProgressState.Locked);
            AssertEntry(save, "b", LevelProgressState.Unlocked);
            AssertEntry(save, "c", LevelProgressState.Completed);
        }

        [Test]
        public void ToState_EmptySaveWithLevels_FirstLevelUnlockedRestLocked()
        {
            Level level0 = CreateLevel("id0");
            Level level1 = CreateLevel("id1");
            var levels = new List<Level> { level0, level1 };
            var handler = new LevelProgressSaveHandler();
            LevelCompletionState state = handler.ToState(new LevelProgressSave(), levels);
            Assert.That(state.GetState("id0"), Is.EqualTo(LevelProgressState.Unlocked));
            Assert.That(state.GetState("id1"), Is.EqualTo(LevelProgressState.Locked));
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(level1);
        }

        [Test]
        public void ToState_SaveWithCompletedLevel_LevelIsCompleted()
        {
            Level level0 = CreateLevel("id0");
            var levels = new List<Level> { level0 };
            var save = new LevelProgressSave { Entries = new[] { new LevelProgressEntry { LevelId = "id0", State = (int)LevelProgressState.Completed } } };
            var handler = new LevelProgressSaveHandler();
            LevelCompletionState state = handler.ToState(save, levels);
            Assert.That(state.GetState("id0"), Is.EqualTo(LevelProgressState.Completed));
            Object.DestroyImmediate(level0);
        }

        [Test]
        public void ToState_SaveWithUnlockedSecondLevel_SecondLevelIsUnlocked()
        {
            Level level0 = CreateLevel("id0");
            Level level1 = CreateLevel("id1");
            var levels = new List<Level> { level0, level1 };
            var save = new LevelProgressSave { Entries = new[] { new LevelProgressEntry { LevelId = "id0", State = (int)LevelProgressState.Completed }, new LevelProgressEntry { LevelId = "id1", State = (int)LevelProgressState.Unlocked } } };
            var handler = new LevelProgressSaveHandler();
            LevelCompletionState state = handler.ToState(save, levels);
            Assert.That(state.GetState("id0"), Is.EqualTo(LevelProgressState.Completed));
            Assert.That(state.GetState("id1"), Is.EqualTo(LevelProgressState.Unlocked));
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(level1);
        }

        [Test]
        public void ToState_NullLevels_ReturnsEmptyState()
        {
            var save = new LevelProgressSave { Entries = new[] { new LevelProgressEntry { LevelId = "id0", State = (int)LevelProgressState.Completed } } };
            var handler = new LevelProgressSaveHandler();
            LevelCompletionState state = handler.ToState(save, null);
            Assert.That(state.GetState("id0"), Is.EqualTo(LevelProgressState.Locked));
        }

        [Test]
        public void ToPersistence_ThenToState_PreservesCompletedAndUnlocked()
        {
            Level level0 = CreateLevel("id0");
            Level level1 = CreateLevel("id1");
            Level level2 = CreateLevel("id2");
            var levels = new List<Level> { level0, level1, level2 };
            var stateBefore = new LevelCompletionState();
            stateBefore.SetState("id0", LevelProgressState.Completed);
            stateBefore.SetState("id1", LevelProgressState.Unlocked);
            stateBefore.SetState("id2", LevelProgressState.Locked);
            var handler = new LevelProgressSaveHandler();
            LevelProgressSave save = handler.ToPersistence(stateBefore);
            LevelCompletionState stateAfter = handler.ToState(save, levels);
            Assert.That(stateAfter.GetState("id0"), Is.EqualTo(LevelProgressState.Completed));
            Assert.That(stateAfter.GetState("id1"), Is.EqualTo(LevelProgressState.Unlocked));
            Assert.That(stateAfter.GetState("id2"), Is.EqualTo(LevelProgressState.Locked));
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(level1);
            Object.DestroyImmediate(level2);
        }

        private static void AssertEntry(LevelProgressSave save, string levelId, LevelProgressState expectedState)
        {
            LevelProgressEntry entry = FindEntry(save, levelId);
            Assert.That(entry, Is.Not.Null);
            Assert.That(entry.State, Is.EqualTo((int)expectedState));
        }

        private static LevelProgressEntry FindEntry(LevelProgressSave save, string levelId)
        {
            if (save?.Entries == null) return null;
            foreach (LevelProgressEntry e in save.Entries)
            {
                if (e != null && e.LevelId == levelId) return e;
            }
            return null;
        }

        private static Level CreateLevel(string levelId)
        {
            Level level = ScriptableObject.CreateInstance<Level>();
            SetPrivateField(level, "levelId", levelId);
            return level;
        }

        private static LevelCompletionState CreateState(params Level[] levels)
        {
            var list = new List<Level>(levels ?? System.Array.Empty<Level>());
            var handler = new LevelProgressSaveHandler();
            return handler.ToState(new LevelProgressSave(), list);
        }

        private static void SetPrivateField(object target, string name, object value)
        {
            FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }

        private static LevelRegistry CreateRegistry(params Level[] levels)
        {
            LevelRegistry registry = ScriptableObject.CreateInstance<LevelRegistry>();
            FieldInfo field = typeof(LevelRegistry).GetField("levels", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(registry, levels ?? System.Array.Empty<Level>());
            return registry;
        }
    }
}
