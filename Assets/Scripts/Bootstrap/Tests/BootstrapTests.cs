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
            SetPrivateField(bootstrap, "views", new View[0]);
            bootstrap.Build();
            INavigation navigation = GetPrivateField<INavigation>(bootstrap, "navigation");
            Assert.That(navigation, Is.Not.Null);
            Assert.That(navigation.StackCount, Is.EqualTo(0));
            Object.DestroyImmediate(bootstrapObject);
        }

        private static void SetPrivateField(object target, string name, object value)
        {
            FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }

        private static T GetPrivateField<T>(object target, string name)
        {
            FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return (T)field.GetValue(target);
        }
    }
}
