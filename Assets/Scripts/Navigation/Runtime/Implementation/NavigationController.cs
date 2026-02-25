using System;
using System.Collections.Generic;

namespace CardMatch.Navigation
{
    public class NavigationController : INavigation
    {
        private readonly Dictionary<Type, IView> views;
        private readonly Stack<IView> stack;

        public int StackCount
        {
            get
            {
                return stack.Count;
            }
        }

        public IView CurrentView
        {
            get
            {
                if (stack.Count == 0)
                {
                    return null;
                }
                return stack.Peek();
            }
        }

        public NavigationController(params IView[] viewArray)
        {
            views = new Dictionary<Type, IView>();
            stack = new Stack<IView>();
            Build(viewArray);
        }

        public void Build(params IView[] viewArray)
        {
            for (int i = 0; i < viewArray.Length; i++)
            {
                IView view = viewArray[i];
                if (view == null)
                {
                    continue;
                }
                Type viewType = view.GetType();
                views[viewType] = view;
            }
        }

        public void Open<T>() where T : IView
        {
            if (!TryGetView<T>(out IView nextView))
            {
                return;
            }
            if (IsCurrentView(nextView))
            {
                return;
            }
            HideCurrentIfAny();
            nextView.Show();
            stack.Push(nextView);
        }

        public void GoTo<T>() where T : IView
        {
            Open<T>();
        }

        public void GoBack()
        {
            if (!CanGoBack())
            {
                return;
            }
            PopCurrentAndShowPrevious();
        }

        private bool TryGetView<T>(out IView nextView) where T : IView
        {
            Type viewType = typeof(T);
            return views.TryGetValue(viewType, out nextView);
        }

        private bool IsCurrentView(IView view)
        {
            if (stack.Count == 0)
            {
                return false;
            }
            return stack.Peek() == view;
        }

        private void HideCurrentIfAny()
        {
            if (stack.Count == 0)
            {
                return;
            }
            IView currentView = stack.Peek();
            currentView.Hide();
        }

        private bool CanGoBack()
        {
            return stack.Count > 1;
        }

        private void PopCurrentAndShowPrevious()
        {
            IView currentView = stack.Pop();
            currentView.Close();
            IView previousView = stack.Peek();
            previousView.Show();
        }
    }
}
