namespace CardMatch.Navigation
{
    public interface INavigation
    {
        int StackCount { get; }
        IView CurrentView { get; }
        void Open<T>(T context, bool closeCurrent = false) where T : IViewContext;
        void GoBack();
        void Focus(IView view);
    }
}
