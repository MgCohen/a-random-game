using System;
using System.Collections.Generic;

namespace CardMatch.Navigation
{
    public class NavigationController : INavigation
    {
        private readonly Dictionary<Type, IView> contextToView;
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

        public NavigationController(params IView[] views)
        {
            contextToView = new Dictionary<Type, IView>();
            stack = new Stack<IView>();
            Build(views);
        }

        private void Build(params IView[] viewArray)
        {
            for (int i = 0; i < viewArray.Length; i++)
            {
                IView view = viewArray[i];
                if (view == null)
                {
                    continue;
                }
                contextToView[view.ContextType] = view;
            }
        }

        public void Open<T>(T context) where T: IViewContext
        {
            if (context == null)
            {
                return;
            }
            if (!contextToView.TryGetValue(context.GetType(), out IView nextView))
            {
                return;
            }
            if (IsCurrentView(nextView))
            {
                return;
            }
            nextView.SetContext(context);
            HideCurrentIfAny();
            nextView.Show();
            stack.Push(nextView);
        }

        public void GoBack()
        {
            if (!CanGoBack())
            {
                return;
            }
            PopCurrentAndShowPrevious();
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
