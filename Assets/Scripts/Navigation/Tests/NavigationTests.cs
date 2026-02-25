using System;
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
            nav.Open(new FakeContextA());
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
            nav.Open(new FakeContextA());
            nav.Open(new FakeContextB());
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
            nav.Open(new FakeContextA());
            nav.Open(new FakeContextB());
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
            nav.Open(new FakeContextA());
            nav.GoBack();
            Assert.That(nav.StackCount, Is.EqualTo(1));
            Assert.That(nav.CurrentView, Is.SameAs(viewA));
            Assert.That(viewA.Status, Is.EqualTo(ViewStatus.Open));
        }

        [Test]
        public void Open_SameContextTwice_DoesNotPushAgain_StackCountStaysOne()
        {
            var viewA = new FakeViewA();
            INavigation nav = new NavigationController(viewA);
            nav.Open(new FakeContextA());
            nav.Open(new FakeContextA());
            Assert.That(nav.StackCount, Is.EqualTo(1));
            Assert.That(nav.CurrentView, Is.SameAs(viewA));
            Assert.That(viewA.Status, Is.EqualTo(ViewStatus.Open));
        }

        [Test]
        public void Open_WhenContextTypeNotRegistered_StackUnchanged()
        {
            var viewA = new FakeViewA();
            INavigation nav = new NavigationController(viewA);
            nav.Open(new FakeContextA());
            nav.Open(new FakeContextB());
            Assert.That(nav.StackCount, Is.EqualTo(1));
            Assert.That(nav.CurrentView, Is.SameAs(viewA));
            Assert.That(viewA.Status, Is.EqualTo(ViewStatus.Open));
        }

        [Test]
        public void Constructor_IgnoresNullEntries()
        {
            var viewA = new FakeViewA();
            INavigation nav = new NavigationController(viewA, null);
            nav.Open(new FakeContextA());
            Assert.That(nav.StackCount, Is.EqualTo(1));
            Assert.That(nav.CurrentView, Is.SameAs(viewA));
        }

        [Test]
        public void Open_TwoContexts_StackCountTwo_SecondIsCurrent_FirstHidden()
        {
            var viewA = new FakeViewA();
            var viewB = new FakeViewB();
            INavigation nav = new NavigationController(viewA, viewB);
            nav.Open(new FakeContextA());
            nav.Open(new FakeContextB());
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
            nav.Open(new FakeContextA());
            nav.Open(new FakeContextB());
            nav.Open(new FakeContextC());
            nav.GoBack();
            nav.GoBack();
            Assert.That(nav.StackCount, Is.EqualTo(1));
            Assert.That(nav.CurrentView, Is.SameAs(viewA));
            Assert.That(viewA.Status, Is.EqualTo(ViewStatus.Open));
            Assert.That(viewB.Status, Is.EqualTo(ViewStatus.Closed));
            Assert.That(viewC.Status, Is.EqualTo(ViewStatus.Closed));
        }

        [Test]
        public void Open_WithContext_ResolvesView_SetsContext_StackCountOne_CurrentViewOpen()
        {
            var context = new FakeViewContext();
            var viewWithContext = new FakeViewWithContext();
            INavigation nav = new NavigationController(viewWithContext);
            nav.Open(context);
            Assert.That(nav.StackCount, Is.EqualTo(1));
            Assert.That(nav.CurrentView, Is.SameAs(viewWithContext));
            Assert.That(viewWithContext.Status, Is.EqualTo(ViewStatus.Open));
            Assert.That(viewWithContext.Context, Is.SameAs(context));
        }

        [Test]
        public void Open_WithContext_WhenNoViewForContext_StackUnchanged()
        {
            var viewA = new FakeViewA();
            INavigation nav = new NavigationController(viewA);
            var context = new FakeViewContext();
            nav.Open(context);
            Assert.That(nav.StackCount, Is.EqualTo(0));
            Assert.That(nav.CurrentView, Is.Null);
        }

        [Test]
        public void Open_WithNullContext_DoesNothing()
        {
            var viewWithContext = new FakeViewWithContext();
            INavigation nav = new NavigationController(viewWithContext);
            nav.Open<FakeViewContext>(null);
            Assert.That(nav.StackCount, Is.EqualTo(0));
            Assert.That(nav.CurrentView, Is.Null);
        }

        private sealed class FakeViewContext : IViewContext
        {
        }

        private sealed class FakeContextA : IViewContext { }
        private sealed class FakeContextB : IViewContext { }
        private sealed class FakeContextC : IViewContext { }

        private abstract class FakeViewBase<TContext> : IView<TContext> where TContext : IViewContext
        {
            public ViewStatus Status { get; private set; } = ViewStatus.Closed;
            public Type ContextType => typeof(TContext);
            public TContext Context { get; private set; }

            public void SetContext(TContext context)
            {
                Context = context;
            }

            public void SetContext(IViewContext context)
            {
                if (context is not TContext typedContext)
                {
                    throw new ArgumentException($"Expected context of type {typeof(TContext).Name}.");
                }
                SetContext(typedContext);
            }

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

        private sealed class FakeViewWithContext : FakeViewBase<FakeViewContext>
        {
        }

        private sealed class FakeViewA : FakeViewBase<FakeContextA>
        {
        }

        private sealed class FakeViewB : FakeViewBase<FakeContextB>
        {
        }

        private sealed class FakeViewC : FakeViewBase<FakeContextC>
        {
        }

    }
}
