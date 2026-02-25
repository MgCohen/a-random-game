using NUnit.Framework;

namespace CardMatch.Persistence.Tests
{
    public class PlayerPrefsPersistenceTests
    {
        private const string TestKeyPrefix = "CardMatch.Persistence.Tests.";
        private IPersistence persistence;

        [SetUp]
        public void SetUp()
        {
            persistence = new PlayerPrefsPersistence();
            persistence.ClearAll();
        }

        [Test]
        public void SaveThenLoad_WithCustomKey_RoundTrips()
        {
            string key = TestKeyPrefix + "RoundTrip";
            var saved = new TestData(42, "foo");
            persistence.Save(saved, key);
            TestData loaded = persistence.Load<TestData>(key);
            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Value, Is.EqualTo(42));
            Assert.That(loaded.Name, Is.EqualTo("foo"));
        }

        [Test]
        public void SaveThenLoad_WithDefaultKey_RoundTrips()
        {
            var saved = new TestData(1, "defaultKey");
            persistence.Save(saved);
            TestData loaded = persistence.Load<TestData>();
            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Value, Is.EqualTo(1));
            Assert.That(loaded.Name, Is.EqualTo("defaultKey"));
        }

        [Test]
        public void ClearAll_RemovesSavedData()
        {
            string key = TestKeyPrefix + "ClearAll";
            persistence.Save(new TestData(10, "clear"), key);
            persistence.ClearAll();
            TestData loaded = persistence.Load<TestData>(key);
            Assert.That(loaded, Is.Null);
        }

        [Test]
        public void Load_WhenKeyMissing_ReturnsDefault()
        {
            string key = TestKeyPrefix + "Missing";
            TestData loaded = persistence.Load<TestData>(key);
            Assert.That(loaded, Is.Null);
        }

        private record TestData(int Value, string Name);
    }
}
