using System;
using System.Collections.Generic;
using CardMatch.Audio;
using UnityEngine;

namespace CardMatch.Navigation
{
    public class NavigationController : INavigation
    {
        private readonly Dictionary<Type, IView> contextToView;
        private readonly Stack<IView> stack;
        private readonly IAudioService transitionAudio;
        private readonly AudioClip transitionClip;
        /// <summary>When non-null, GoBack with no previous view on stack will open this view (e.g. after Open(closeCurrent: true)).</summary>
        private IView lastClosedView;

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

        /// <summary>Constructor for tests and callers that do not use transition sound. Chains to the full constructor with null audio.</summary>
        public NavigationController(params IView[] views) : this(null, null, views)
        {
        }

        public NavigationController(IAudioService transitionAudio, AudioClip transitionClip, params IView[] views)
        {
            contextToView = new Dictionary<Type, IView>();
            stack = new Stack<IView>();
            this.transitionAudio = transitionAudio;
            this.transitionClip = transitionClip;
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
                view.SetNavigation(this);
            }
        }

        public void Open<T>(T context, bool closeCurrent = false) where T: IViewContext
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
            if (IsOnStack(nextView))
            {
                PopToViewAndFocus(nextView, context);
                PlayTransitionSound();
                return;
            }
            if (closeCurrent && stack.Count > 0)
            {
                IView current = stack.Pop();
                lastClosedView = current;
                current.Close();
            }
            else
            {
                HideCurrentIfAny();
            }
            nextView.SetContext(context);
            nextView.Open();
            stack.Push(nextView);
            PlayTransitionSound();
        }

        public void GoBack()
        {
            if (CanGoBack())
            {
                PopCurrentAndShowPrevious();
                return;
            }
            if (stack.Count > 0 && lastClosedView != null)
            {
                RestoreLastClosedView();
            }
        }

        private bool IsCurrentView(IView view)
        {
            if (stack.Count == 0)
            {
                return false;
            }
            return stack.Peek() == view;
        }

        private bool IsOnStack(IView view)
        {
            foreach (IView v in stack)
            {
                if (v == view)
                    return true;
            }
            return false;
        }

        private void PopToViewAndFocus(IView view, IViewContext context)
        {
            List<IView> popped = new List<IView>();
            while (stack.Count > 0 && stack.Peek() != view)
            {
                popped.Add(stack.Pop());
            }
            if (stack.Count == 0)
            {
                return;
            }
            foreach (IView v in popped)
            {
                v.Close();
            }
            view.SetContext(context);
            view.Focus();
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
            previousView.Focus();
            PlayTransitionSound();
        }

        private void RestoreLastClosedView()
        {
            IView currentView = stack.Pop();
            currentView.Close();
            IView viewToRestore = lastClosedView;
            lastClosedView = currentView;
            viewToRestore.Open();
            stack.Push(viewToRestore);
            PlayTransitionSound();
        }

        private void PlayTransitionSound()
        {
            if (transitionAudio != null && transitionClip != null)
                transitionAudio.PlaySound(transitionClip);
        }

        public void Focus(IView view)
        {
            if (view == null || view.Status != ViewStatus.Hidden)
            {
                return;
            }
            List<IView> popped = new List<IView>();
            while (stack.Count > 0 && stack.Peek() != view)
            {
                popped.Add(stack.Pop());
            }
            if (stack.Count == 0)
            {
                for (int j = popped.Count - 1; j >= 0; j--)
                {
                    stack.Push(popped[j]);
                }
                return;
            }
            foreach (IView v in popped)
            {
                v.Close();
            }
            view.Focus();
        }
    }
}
