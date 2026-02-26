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
        private AudioSource sfxSource;

        [SetUp]
        public void SetUp()
        {
            serviceObject = new GameObject("AudioService");
            service = serviceObject.AddComponent<AudioService>();

            var sfxObject = new GameObject("SfxSource");
            sfxSource = sfxObject.AddComponent<AudioSource>();
            sfxObject.transform.SetParent(serviceObject.transform);

            SetPrivateField(service, "sfxSource", sfxSource);
            SetPrivateField(service, "maxConcurrentSounds", 3);
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
        public void SetMute_WithNullSources_DoesNotThrow()
        {
            SetPrivateField(service, "sfxSource", null);
            Assert.DoesNotThrow(() => service.SetMute(true));
        }

        [Test]
        public void SetMute_True_SetsBothSourcesMute()
        {
            service.SetMute(true);
            var pool = GetPrivateField(service, "_pool") as System.Collections.IList;
            Assert.That(pool, Is.Not.Null);
            foreach (AudioSource source in pool)
                Assert.That(source.mute, Is.True, "Each pool source should be muted");
        }

        [Test]
        public void SetMute_False_UnmutesBothSources()
        {
            service.SetMute(true);
            service.SetMute(false);
            var pool = GetPrivateField(service, "_pool") as System.Collections.IList;
            Assert.That(pool, Is.Not.Null);
            foreach (AudioSource source in pool)
                Assert.That(source.mute, Is.False, "Each pool source should be unmuted");
        }

        private static void SetPrivateField(object target, string name, object value)
        {
            var field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Field '{name}' not found");
            field.SetValue(target, value);
        }

        private static object GetPrivateField(object target, string name)
        {
            var field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Field '{name}' not found");
            return field.GetValue(target);
        }

        private static AudioClip CreateDummyClip()
        {
            return AudioClip.Create("TestClip", 44100, 1, 44100, false);
        }
    }
}
