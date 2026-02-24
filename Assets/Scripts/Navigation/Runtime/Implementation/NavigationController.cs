using System;
using System.Collections.Generic;

namespace CardMatch.Navigation
{
    public class NavigationController : INavigation
    {
        private readonly Dictionary<Type, IView> views = new Dictionary<Type, IView>();
        private readonly Stack<IView> stack = new Stack<IView>();

        public int StackCount => stack.Count;

        public IView CurrentView => stack.Count > 0 ? stack.Peek() : null;

        public NavigationController(params IView[] viewArray)
        {
            for (int i = 0; i < viewArray.Length; i++)
            {
                IView v = viewArray[i];
                if (v == null) continue;
                Type key = v.GetType();
                views[key] = v;
            }
        }

        public void Open<T>() where T : IView
        {
            if (!TryGetView<T>(out IView next))
            {
                return;
            }
            if (IsCurrent(next))
            {
                return;
            }
            HideCurrentIfAny();
            next.Show();
            stack.Push(next);
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

        private bool TryGetView<T>(out IView next) where T : IView
        {
            Type key = typeof(T);
            return views.TryGetValue(key, out next);
        }

        private bool IsCurrent(IView view)
        {
            bool hasAny = stack.Count > 0;
            if (!hasAny)
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
            IView current = stack.Peek();
            current.Hide();
        }

        private bool CanGoBack()
        {
            return stack.Count > 1;
        }

        private void PopCurrentAndShowPrevious()
        {
            IView current = stack.Pop();
            current.Close();
            IView previous = stack.Peek();
            previous.Show();
        }
    }
}
