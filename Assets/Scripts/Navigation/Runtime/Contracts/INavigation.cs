namespace CardMatch.Navigation
{
    public interface INavigation
    {
        int StackCount { get; }
        IView CurrentView { get; }
        void Open(IViewContext context);
        void GoBack();
    }
}
