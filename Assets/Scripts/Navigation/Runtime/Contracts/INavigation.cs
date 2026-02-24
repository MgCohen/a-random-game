namespace CardMatch.Navigation
{
    public interface INavigation
    {
        int StackCount { get; }
        IView CurrentView { get; }
        void Open<T>() where T : IView;
        void GoTo<T>() where T : IView;
        void GoBack();
    }
}
