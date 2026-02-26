using CardMatch.Audio;
using CardMatch.Navigation;

namespace CardMatch.Settings
{
    public class SettingsViewContext : IViewContext
    {
        public INavigation Navigation { get; }
        public IAudioService AudioService { get; }

        public SettingsViewContext(INavigation navigation, IAudioService audioService)
        {
            Navigation = navigation;
            AudioService = audioService;
        }
    }
}
