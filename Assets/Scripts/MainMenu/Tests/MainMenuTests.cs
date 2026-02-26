using System.Collections.Generic;
using System.Reflection;
using CardMatch.Audio;
using CardMatch.CardMatch;
using CardMatch.Levels;
using CardMatch.Navigation;
using CardMatch.PlaySystem;
using CardMatch.Settings;
using NUnit.Framework;
using UnityEngine;

namespace CardMatch.MainMenu.Tests
{
    public class MainMenuTests
    {
        [Test]
        public void GetLevelEntries_WhenNoLevels_ReturnsEmptyList()
        {
            LevelRegistry registry = CreateRegistry();
            ILevelController levelController = new LevelController(registry, null);
            var nav = new FakeNavigation();
            var playSystem = new FakePlaySystem();
            var audio = new FakeAudioService();
            var ctx = new MainMenuViewContext(levelController, nav, playSystem, audio);
            IReadOnlyList<MainMenuLevelEntry> entries = ctx.GetLevelEntries();
            Assert.That(entries.Count, Is.EqualTo(0));
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void GetLevelEntries_WhenOneLevel_ReturnsOneEntry_FirstLevelUnlocked()
        {
            Level level0 = CreateLevel("id0");
            LevelRegistry registry = CreateRegistry(level0);
            ILevelController levelController = new LevelController(registry, null);
            var nav = new FakeNavigation();
            var playSystem = new FakePlaySystem();
            var audio = new FakeAudioService();
            var ctx = new MainMenuViewContext(levelController, nav, playSystem, audio);
            IReadOnlyList<MainMenuLevelEntry> entries = ctx.GetLevelEntries();
            Assert.That(entries.Count, Is.EqualTo(1));
            Assert.That(entries[0].Level, Is.SameAs(level0));
            Assert.That(entries[0].State, Is.EqualTo(LevelProgressState.Unlocked));
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void CanSelect_UnlockedLevel_ReturnsTrue()
        {
            Level level0 = CreateLevel("id0");
            LevelRegistry registry = CreateRegistry(level0);
            ILevelController levelController = new LevelController(registry, null);
            var nav = new FakeNavigation();
            var playSystem = new FakePlaySystem();
            var audio = new FakeAudioService();
            var ctx = new MainMenuViewContext(levelController, nav, playSystem, audio);
            Assert.That(ctx.CanSelect(level0), Is.True);
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void CanSelect_LockedLevel_ReturnsFalse()
        {
            Level level0 = CreateLevel("id0");
            Level level1 = CreateLevel("id1");
            LevelRegistry registry = CreateRegistry(level0, level1);
            ILevelController levelController = new LevelController(registry, null);
            var nav = new FakeNavigation();
            var playSystem = new FakePlaySystem();
            var audio = new FakeAudioService();
            var ctx = new MainMenuViewContext(levelController, nav, playSystem, audio);
            Assert.That(ctx.CanSelect(level1), Is.False);
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(level1);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void SelectLevel_WhenUnlocked_SetsSelectedLevel()
        {
            Level level0 = CreateLevel("id0");
            LevelRegistry registry = CreateRegistry(level0);
            ILevelController levelController = new LevelController(registry, null);
            var nav = new FakeNavigation();
            var playSystem = new FakePlaySystem();
            var audio = new FakeAudioService();
            var ctx = new MainMenuViewContext(levelController, nav, playSystem, audio);
            Assert.That(ctx.SelectedLevel, Is.Null);
            ctx.SelectLevel(level0);
            Assert.That(ctx.SelectedLevel, Is.SameAs(level0));
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void SelectLevel_WhenLocked_DoesNotSetSelectedLevel()
        {
            Level level0 = CreateLevel("id0");
            Level level1 = CreateLevel("id1");
            LevelRegistry registry = CreateRegistry(level0, level1);
            ILevelController levelController = new LevelController(registry, null);
            var nav = new FakeNavigation();
            var playSystem = new FakePlaySystem();
            var audio = new FakeAudioService();
            var ctx = new MainMenuViewContext(levelController, nav, playSystem, audio);
            ctx.SelectLevel(level1);
            Assert.That(ctx.SelectedLevel, Is.Null);
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(level1);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void OnPlayClicked_WhenSelectedLevelExists_CallsPlaySystemWithSelectedLevel()
        {
            Level level0 = CreateLevel("id0");
            LevelRegistry registry = CreateRegistry(level0);
            ILevelController levelController = new LevelController(registry, null);
            var nav = new FakeNavigation();
            var playSystem = new FakePlaySystem();
            var audio = new FakeAudioService();
            var ctx = new MainMenuViewContext(levelController, nav, playSystem, audio);
            ctx.SelectLevel(level0);
            ctx.PlaySelected();
            Assert.That(playSystem.PlayCalledWith, Is.SameAs(level0));
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void OnPlayClicked_WhenNoSelection_DoesNotCallPlaySystem()
        {
            Level level0 = CreateLevel("id0");
            LevelRegistry registry = CreateRegistry(level0);
            ILevelController levelController = new LevelController(registry, null);
            var nav = new FakeNavigation();
            var playSystem = new FakePlaySystem();
            var audio = new FakeAudioService();
            var ctx = new MainMenuViewContext(levelController, nav, playSystem, audio);
            ctx.PlaySelected();
            Assert.That(playSystem.PlayCalled, Is.False);
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(registry);
        }

        [Test]
        public void OnSettingsClicked_OpensSettingsViewWithNavigationAndAudio()
        {
            Level level0 = CreateLevel("id0");
            LevelRegistry registry = CreateRegistry(level0);
            ILevelController levelController = new LevelController(registry, null);
            var nav = new FakeNavigation();
            var playSystem = new FakePlaySystem();
            var audio = new FakeAudioService();
            var ctx = new MainMenuViewContext(levelController, nav, playSystem, audio);
            ctx.OnSettingsClicked();
            SettingsViewContext settingsContext = nav.OpenedContext as SettingsViewContext;
            Assert.That(settingsContext, Is.Not.Null);
            Assert.That(settingsContext.Navigation, Is.SameAs(nav));
            Assert.That(settingsContext.AudioService, Is.SameAs(audio));
            Object.DestroyImmediate(level0);
            Object.DestroyImmediate(registry);
        }

        private static Level CreateLevel(string levelId)
        {
            Level level = ScriptableObject.CreateInstance<Level>();
            SetPrivateField(level, "levelId", levelId);
            return level;
        }

        private static LevelRegistry CreateRegistry(params Level[] levels)
        {
            LevelRegistry registry = ScriptableObject.CreateInstance<LevelRegistry>();
            FieldInfo field = typeof(LevelRegistry).GetField("levels", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(registry, levels ?? System.Array.Empty<Level>());
            return registry;
        }

        private static void SetPrivateField(object target, string name, object value)
        {
            FieldInfo f = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(f, Is.Not.Null);
            f.SetValue(target, value);
        }

        private sealed class FakeNavigation : INavigation
        {
            public int StackCount => 0;
            public IView CurrentView => null;
            public IViewContext OpenedContext { get; private set; }
            public void GoBack() { }
            public void Open<T>(T context, bool closeCurrent = false) where T : IViewContext
            {
                OpenedContext = context;
            }
            public void Focus(IView view) { }
        }

        private sealed class FakePlaySystem : IPlaySystem
        {
            public Level PlayCalledWith { get; private set; }
            public bool PlayCalled => PlayCalledWith != null;

            public void Play(Level level) { PlayCalledWith = level; }
            public void GoBack() { }
            public void MarkCompleted(Level level) { }
        }

        private sealed class FakeAudioService : IAudioService
        {
            public bool IsMuted { get; private set; }
            public void SetMute(bool mute)
            {
                IsMuted = mute;
            }
            public void PlaySound(AudioClip clip) { }
        }
    }
}
