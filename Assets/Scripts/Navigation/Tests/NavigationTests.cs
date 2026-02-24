using CardMatch.Navigation;
using NUnit.Framework;

namespace CardMatch.Navigation.Tests
{
    public class NavigationTests
    {
        [Test]
        public void StackCount_Initially_IsZero()
        {
            INavigation nav = new NavigationController();
            Assert.That(nav.StackCount, Is.EqualTo(0));
        }

        [Test]
        public void CurrentView_Initially_IsNull()
        {
            INavigation nav = new NavigationController();
            Assert.That(nav.CurrentView, Is.Null);
        }

        [Test]
        public void Constructor_ThenOpen_StackCountIsOne_CurrentViewIsOpen_ViewStatusIsOpen()
        {
            var viewA = new FakeViewA();
            INavigation nav = new NavigationController(viewA);
            nav.Open<FakeViewA>();
            Assert.That(nav.StackCount, Is.EqualTo(1));
            Assert.That(nav.CurrentView, Is.SameAs(viewA));
            Assert.That(viewA.Status, Is.EqualTo(ViewStatus.Open));
        }

        [Test]
        public void Open_ThenOpenAnother_StackCountIsTwo_FirstIsHidden_SecondIsOpen()
        {
            var viewA = new FakeViewA();
            var viewB = new FakeViewB();
            INavigation nav = new NavigationController(viewA, viewB);
            nav.Open<FakeViewA>();
            nav.Open<FakeViewB>();
            Assert.That(nav.StackCount, Is.EqualTo(2));
            Assert.That(nav.CurrentView, Is.SameAs(viewB));
            Assert.That(viewA.Status, Is.EqualTo(ViewStatus.Hidden));
            Assert.That(viewB.Status, Is.EqualTo(ViewStatus.Open));
        }

        [Test]
        public void GoBack_AfterTwoOpen_StackCountIsOne_PreviousIsOpen_PoppedIsClosed()
        {
            var viewA = new FakeViewA();
            var viewB = new FakeViewB();
            INavigation nav = new NavigationController(viewA, viewB);
            nav.Open<FakeViewA>();
            nav.Open<FakeViewB>();
            nav.GoBack();
            Assert.That(nav.StackCount, Is.EqualTo(1));
            Assert.That(nav.CurrentView, Is.SameAs(viewA));
            Assert.That(viewA.Status, Is.EqualTo(ViewStatus.Open));
            Assert.That(viewB.Status, Is.EqualTo(ViewStatus.Closed));
        }

        [Test]
        public void GoBack_WhenStackCountOne_DoesNothing()
        {
            var viewA = new FakeViewA();
            INavigation nav = new NavigationController(viewA);
            nav.Open<FakeViewA>();
            nav.GoBack();
            Assert.That(nav.StackCount, Is.EqualTo(1));
            Assert.That(nav.CurrentView, Is.SameAs(viewA));
            Assert.That(viewA.Status, Is.EqualTo(ViewStatus.Open));
        }

        [Test]
        public void Open_SameTypeTwice_DoesNotPushAgain_StackCountStaysOne()
        {
            var viewA = new FakeViewA();
            INavigation nav = new NavigationController(viewA);
            nav.Open<FakeViewA>();
            nav.Open<FakeViewA>();
            Assert.That(nav.StackCount, Is.EqualTo(1));
            Assert.That(nav.CurrentView, Is.SameAs(viewA));
            Assert.That(viewA.Status, Is.EqualTo(ViewStatus.Open));
        }

        [Test]
        public void Open_WhenTypeNotRegistered_StackUnchanged()
        {
            var viewA = new FakeViewA();
            INavigation nav = new NavigationController(viewA);
            nav.Open<FakeViewA>();
            nav.Open<FakeViewB>();
            Assert.That(nav.StackCount, Is.EqualTo(1));
            Assert.That(nav.CurrentView, Is.SameAs(viewA));
            Assert.That(viewA.Status, Is.EqualTo(ViewStatus.Open));
        }

        [Test]
        public void Constructor_IgnoresNullEntries()
        {
            var viewA = new FakeViewA();
            INavigation nav = new NavigationController(viewA, null);
            nav.Open<FakeViewA>();
            Assert.That(nav.StackCount, Is.EqualTo(1));
            Assert.That(nav.CurrentView, Is.SameAs(viewA));
        }

        [Test]
        public void GoTo_BehavesLikeOpen_StackCountAndStatusCorrect()
        {
            var viewA = new FakeViewA();
            var viewB = new FakeViewB();
            INavigation nav = new NavigationController(viewA, viewB);
            nav.GoTo<FakeViewA>();
            nav.GoTo<FakeViewB>();
            Assert.That(nav.StackCount, Is.EqualTo(2));
            Assert.That(nav.CurrentView, Is.SameAs(viewB));
            Assert.That(viewA.Status, Is.EqualTo(ViewStatus.Hidden));
            Assert.That(viewB.Status, Is.EqualTo(ViewStatus.Open));
        }

        [Test]
        public void ThreeViews_GoBackTwice_StackCountOne_FirstIsOpen_OthersClosed()
        {
            var viewA = new FakeViewA();
            var viewB = new FakeViewB();
            var viewC = new FakeViewC();
            INavigation nav = new NavigationController(viewA, viewB, viewC);
            nav.Open<FakeViewA>();
            nav.Open<FakeViewB>();
            nav.Open<FakeViewC>();
            nav.GoBack();
            nav.GoBack();
            Assert.That(nav.StackCount, Is.EqualTo(1));
            Assert.That(nav.CurrentView, Is.SameAs(viewA));
            Assert.That(viewA.Status, Is.EqualTo(ViewStatus.Open));
            Assert.That(viewB.Status, Is.EqualTo(ViewStatus.Closed));
            Assert.That(viewC.Status, Is.EqualTo(ViewStatus.Closed));
        }

        private sealed class FakeViewA : IView
        {
            public ViewStatus Status { get; private set; } = ViewStatus.Closed;

            public void Show()
            {
                Status = ViewStatus.Open;
            }

            public void Hide()
            {
                Status = ViewStatus.Hidden;
            }

            public void Close()
            {
                Status = ViewStatus.Closed;
            }
        }

        private sealed class FakeViewB : IView
        {
            public ViewStatus Status { get; private set; } = ViewStatus.Closed;

            public void Show()
            {
                Status = ViewStatus.Open;
            }

            public void Hide()
            {
                Status = ViewStatus.Hidden;
            }

            public void Close()
            {
                Status = ViewStatus.Closed;
            }
        }

        private sealed class FakeViewC : IView
        {
            public ViewStatus Status { get; private set; } = ViewStatus.Closed;

            public void Show()
            {
                Status = ViewStatus.Open;
            }

            public void Hide()
            {
                Status = ViewStatus.Hidden;
            }

            public void Close()
            {
                Status = ViewStatus.Closed;
            }
        }
    }
}
