namespace CardMatch.Navigation
{
    public interface IView
    {
        ViewStatus Status { get; }
        void Show();
        void Hide();
        void Close();
    }
}
