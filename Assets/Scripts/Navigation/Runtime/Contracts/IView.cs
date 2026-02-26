using System;

namespace CardMatch.Navigation
{
    public interface IView
    {
        Type ContextType { get; }
        ViewStatus Status { get; }
        void SetContext(IViewContext context);
        void Show();
        void Hide();
        void Close();
        void SetNavigation(INavigation navigation);
        /// <summary>Called by Navigation when the view is being opened (first time on stack).</summary>
        void Open();
        /// <summary>Called by Navigation when the view is being focused (returning from hidden, e.g. GoBack or Open to existing stack entry).</summary>
        void Focus();
        /// <summary>Called by the view to ask Navigation to focus it (e.g. back button).</summary>
        void RequestFocus();
    }
}
