using CardMatch.Navigation;
using CardMatch.PlayView;
using NUnit.Framework;

namespace CardMatch.PlayView.Tests
{
    public class PlayViewTests
    {
        [Test]
        public void PlayView_IsAView()
        {
            Assert.That(typeof(PlayView).IsSubclassOf(typeof(View)), Is.True);
        }
    }
}
