using System.Collections.Generic;
using System.Reflection;
using CardMatch.CardMatch;
using CardMatch.Levels;
using CardMatch.Persistence;
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
            ILevelController controller = new LevelController(registry, null);
            Assert.That(controller.GetLevels().Count, Is.EqualTo(0));
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void GetLevels_WhenRegistryHasLevels_ReturnsRegistryOrder()
        {
            Level level0 = CreateLevel("id0");
            Level level1 = CreateLevel("id1");
            LevelRegistry registry = CreateRegistry(level0, level1);
            ILevelController controller = new LevelController(registry, null);
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
            ILevelController controller = new LevelController(registry, null);
            Assert.That(controller.GetLevel(0), Is.SameAs(level0));
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void GetLevel_OutOfRange_ReturnsNull()
        {
            LevelRegistry registry = CreateRegistry();
            ILevelController controller = new LevelController(registry, null);
            Assert.That(controller.GetLevel(-1), Is.Null);
            Assert.That(controller.GetLevel(0), Is.Null);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void IsUnlocked_FirstLevel_IsTrue()
        {
            Level level0 = CreateLevel("id0");
            LevelRegistry registry = CreateRegistry(level0);
            ILevelController controller = new LevelController(registry, null);
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
            ILevelController controller = new LevelController(registry, null);
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
            ILevelController controller = new LevelController(registry, null);
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
            ILevelController controller = new LevelController(registry, null);
            Assert.That(controller.IsUnlocked(level1), Is.False);
            controller.MarkCompleted(level0);
            Assert.That(controller.IsUnlocked(level1), Is.True);
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(level1);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void LevelCompletionState_NewInstance_HasEmptyLists()
        {
            var state = new LevelCompletionState();
            Assert.That(state.UnlockedLevelIds, Is.Not.Null);
            Assert.That(state.CompletedLevelIds, Is.Not.Null);
            Assert.That(state.UnlockedLevelIds.Count, Is.EqualTo(0));
            Assert.That(state.CompletedLevelIds.Count, Is.EqualTo(0));
        }

        [Test]
        public void MarkCompleted_Persisted_Reload_IsCompletedAndUnlocked()
        {
            Level level0 = CreateLevel("id0");
            Level level1 = CreateLevel("id1");
            LevelRegistry registry = CreateRegistry(level0, level1);
            var persistence = new PlayerPrefsPersistence();
            persistence.ClearAll();
            var controller1 = new LevelController(registry, persistence);
            controller1.MarkCompleted(level0);
            var controller2 = new LevelController(registry, persistence);
            Assert.That(controller2.IsCompleted(level0), Is.True);
            Assert.That(controller2.IsUnlocked(level0), Is.True);
            Assert.That(controller2.IsUnlocked(level1), Is.True);
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(level1);
            Object.DestroyImmediate(registry);
        }

        private static Level CreateLevel(string levelId)
        {
            Level level = ScriptableObject.CreateInstance<Level>();
            SetPrivateField(level, "levelId", levelId);
            return level;
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
