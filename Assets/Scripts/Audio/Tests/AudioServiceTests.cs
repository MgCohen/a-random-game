using System.Reflection;
using CardMatch.Audio;
using NUnit.Framework;
using UnityEngine;

namespace CardMatch.Audio.Tests
{
    public class AudioServiceTests
    {
        private GameObject serviceObject;
        private AudioService service;
        private AudioSource musicSource;
        private AudioSource sfxSource;

        [SetUp]
        public void SetUp()
        {
            serviceObject = new GameObject("AudioService");
            service = serviceObject.AddComponent<AudioService>();

            var musicObject = new GameObject("MusicSource");
            musicSource = musicObject.AddComponent<AudioSource>();
            musicObject.transform.SetParent(serviceObject.transform);

            var sfxObject = new GameObject("SfxSource");
            sfxSource = sfxObject.AddComponent<AudioSource>();
            sfxObject.transform.SetParent(serviceObject.transform);

            SetPrivateField(service, "musicSource", musicSource);
            SetPrivateField(service, "sfxSource", sfxSource);
        }

        [TearDown]
        public void TearDown()
        {
            if (serviceObject != null)
                Object.DestroyImmediate(serviceObject);
        }

        [Test]
        public void PlaySound_WithNullClip_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => service.PlaySound(null));
        }

        [Test]
        public void PlaySound_WithNullSfxSource_DoesNotThrow()
        {
            SetPrivateField(service, "sfxSource", null);
            var clip = CreateDummyClip();
            Assert.DoesNotThrow(() => service.PlaySound(clip));
            Object.DestroyImmediate(clip);
        }

        [Test]
        public void PlaySound_WithValidClipAndSfxSource_DoesNotThrow()
        {
            var clip = CreateDummyClip();
            Assert.DoesNotThrow(() => service.PlaySound(clip));
            Object.DestroyImmediate(clip);
        }

        [Test]
        public void PlayMusic_WithNullMusicSource_DoesNotThrow()
        {
            SetPrivateField(service, "musicSource", null);
            var clip = CreateDummyClip();
            Assert.DoesNotThrow(() => service.PlayMusic(clip));
            Object.DestroyImmediate(clip);
        }

        [Test]
        public void PlayMusic_WithNullClip_StopsAndDoesNotThrow()
        {
            Assert.DoesNotThrow(() => service.PlayMusic(null));
        }

        [Test]
        public void PlayMusic_WithValidClip_SetsClipLoopAndPlays()
        {
            var clip = CreateDummyClip();
            service.PlayMusic(clip);
            Assert.That(musicSource.clip, Is.SameAs(clip));
            Assert.That(musicSource.loop, Is.True);
            Assert.That(musicSource.isPlaying, Is.True);
            Object.DestroyImmediate(clip);
        }

        [Test]
        public void PlayMusic_WhenAlreadyPlaying_SwitchesToNewClip()
        {
            var clip1 = CreateDummyClip();
            var clip2 = CreateDummyClip();
            service.PlayMusic(clip1);
            service.PlayMusic(clip2);
            Assert.That(musicSource.clip, Is.SameAs(clip2));
            Assert.That(musicSource.loop, Is.True);
            Object.DestroyImmediate(clip1);
            Object.DestroyImmediate(clip2);
        }

        [Test]
        public void StopMusic_WithNullMusicSource_DoesNotThrow()
        {
            SetPrivateField(service, "musicSource", null);
            Assert.DoesNotThrow(() => service.StopMusic());
        }

        [Test]
        public void StopMusic_WithMusicSource_StopsPlayback()
        {
            var clip = CreateDummyClip();
            service.PlayMusic(clip);
            Assert.That(musicSource.isPlaying, Is.True);
            service.StopMusic();
            Assert.That(musicSource.isPlaying, Is.False);
            Object.DestroyImmediate(clip);
        }

        [Test]
        public void SetMusicVolume_WithNullMusicSource_DoesNotThrow()
        {
            SetPrivateField(service, "musicSource", null);
            Assert.DoesNotThrow(() => service.SetMusicVolume(0.5f));
        }

        [Test]
        public void SetMusicVolume_ClampsToValidRange()
        {
            service.SetMusicVolume(1.5f);
            Assert.That(musicSource.volume, Is.EqualTo(1f));
            service.SetMusicVolume(-0.5f);
            Assert.That(musicSource.volume, Is.EqualTo(0f));
            service.SetMusicVolume(0.5f);
            Assert.That(musicSource.volume, Is.EqualTo(0.5f));
        }

        [Test]
        public void SetMute_WithNullSources_DoesNotThrow()
        {
            SetPrivateField(service, "musicSource", null);
            SetPrivateField(service, "sfxSource", null);
            Assert.DoesNotThrow(() => service.SetMute(true));
        }

        [Test]
        public void SetMute_True_SetsBothSourcesMute()
        {
            service.SetMute(true);
            Assert.That(musicSource.mute, Is.True);
            Assert.That(sfxSource.mute, Is.True);
        }

        [Test]
        public void SetMute_False_UnmutesBothSources()
        {
            service.SetMute(true);
            service.SetMute(false);
            Assert.That(musicSource.mute, Is.False);
            Assert.That(sfxSource.mute, Is.False);
        }

        [Test]
        public void IsMuted_WhenMuted_ReturnsTrue()
        {
            service.SetMute(true);
            Assert.That(service.IsMuted, Is.True);
        }

        [Test]
        public void IsMuted_WhenUnmuted_ReturnsFalse()
        {
            service.SetMute(false);
            Assert.That(service.IsMuted, Is.False);
        }

        private static void SetPrivateField(object target, string name, object value)
        {
            var field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Field '{name}' not found");
            field.SetValue(target, value);
        }

        private static AudioClip CreateDummyClip()
        {
            return AudioClip.Create("TestClip", 44100, 1, 44100, false);
        }
    }
}
