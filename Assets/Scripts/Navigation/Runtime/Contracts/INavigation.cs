namespace CardMatch.Navigation
{
    public interface INavigation
    {
        int StackCount { get; }
        IView CurrentView { get; }
        void Open<T>(T context) where T : IViewContext;
        void GoBack();
    }
}
