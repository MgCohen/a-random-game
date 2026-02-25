using CardMatch.Navigation;

namespace CardMatch.PlayView
{
    public class PlayViewContext : IViewContext
    {
        public INavigation Navigation { get; }

        public PlayViewContext(INavigation navigation)
        {
            Navigation = navigation;
        }
    }
}
