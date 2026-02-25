using System.Reflection;
using CardMatch.Bootstrap;
using CardMatch.Navigation;
using NUnit.Framework;
using UnityEngine;

namespace CardMatch.Bootstrap.Tests
{
    public class BootstrapTests
    {
        [Test]
        public void Build_ComposesNavigation()
        {
            var bootstrapObject = new GameObject("bootstrap");
            var bootstrap = bootstrapObject.AddComponent<Bootstrap>();
            SetPrivateField(bootstrap, "viewPrefabs", new View[0]);
            bootstrap.Build();
            Assert.That(bootstrap.Navigation, Is.Not.Null);
            Assert.That(bootstrap.Navigation.StackCount, Is.EqualTo(0));
            Object.DestroyImmediate(bootstrapObject);
        }

        private static void SetPrivateField(object target, string name, object value)
        {
            FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }
    }
}
