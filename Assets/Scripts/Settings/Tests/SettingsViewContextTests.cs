using CardMatch.Audio;
using CardMatch.Navigation;
using NUnit.Framework;
using UnityEngine;

namespace CardMatch.Settings.Tests
{
    public class SettingsViewContextTests
    {
        [Test]
        public void Constructor_SetsNavigationAndAudioService()
        {
            var navigation = new FakeNavigation();
            var audioService = new FakeAudioService();
            var context = new SettingsViewContext(navigation, audioService);
            Assert.That(context.Navigation, Is.SameAs(navigation));
            Assert.That(context.AudioService, Is.SameAs(audioService));
        }

        private sealed class FakeNavigation : INavigation
        {
            public int StackCount => 0;
            public IView CurrentView => null;
            public void Open<T>(T context, bool closeCurrent = false) where T : IViewContext { }
            public void GoBack() { }
            public void Focus(IView view) { }
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
