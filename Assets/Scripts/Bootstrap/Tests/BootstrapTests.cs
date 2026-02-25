using System.Reflection;
using CardMatch.Bootstrap;
using CardMatch.Config;
using CardMatch.Navigation;
using NUnit.Framework;
using UnityEngine;

namespace CardMatch.Bootstrap.Tests
{
    public class BootstrapTests
    {
        [Test]
        public void Build_ComposesNavigationAndCardMatchServices()
        {
            var bootstrapObject = new GameObject("bootstrap");
            var bootstrap = bootstrapObject.AddComponent<Bootstrap>();
            Level level = ScriptableObject.CreateInstance<Level>();
            SetPrivateField(level, "config", new LevelConfig
            {
                Layout = new LayoutConfig { Rows = 1, Columns = 2 },
                Scoring = new ScoreRules { BaseMatchPoints = 3, ComboBonusPerLevel = 1 }
            });
            SetPrivateField(bootstrap, "level", level);
            SetPrivateField(bootstrap, "shuffledCardIds", new[] { 7, 7 });
            SetPrivateField(bootstrap, "viewPrefabs", new View[0]);
            bootstrap.Build();
            Assert.That(bootstrap.Navigation, Is.Not.Null);
            Assert.That(bootstrap.MatchEvents, Is.Not.Null);
            Assert.That(bootstrap.Match, Is.Not.Null);
            Assert.That(bootstrap.ScoreService, Is.Not.Null);
            Assert.That(bootstrap.Navigation.StackCount, Is.EqualTo(0));
            Object.DestroyImmediate(bootstrapObject);
            Object.DestroyImmediate(level);
        }

        private static void SetPrivateField(object target, string name, object value)
        {
            FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }
    }
}
