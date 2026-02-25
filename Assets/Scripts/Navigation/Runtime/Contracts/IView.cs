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
    }
}
