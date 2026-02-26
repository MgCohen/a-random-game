using CardMatch.Audio;
using CardMatch.Levels;
using CardMatch.MainMenu;
using CardMatch.Navigation;
using CardMatch.Persistence;
using CardMatch.PlaySystem;
using Play = CardMatch.PlaySystem.PlaySystem;
using UnityEngine;

namespace CardMatch.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private LevelRegistry levelRegistry;
        [SerializeField] private View[] views;
        [SerializeField] private AudioService audioService;
        [SerializeField] private AudioClip transitionClip;

        private IPersistence persistence;
        private INavigation navigation;
        private ILevelController levels;
        private IPlaySystem playSystem;
        private IPlayMatchFactory factory;


        private void Start()
        {
            Build();
        }

        public void Build()
        {
            persistence = new PlayerPrefsPersistence();
            navigation = new NavigationController(audioService, transitionClip, views);
            levels = new LevelController(levelRegistry, persistence);
            factory = new PlayMatchFactory();
            playSystem = new Play(navigation, factory, levels);
            OpenInitialView();
        }

        private void OpenInitialView()
        {
            if (navigation == null || levelRegistry == null)
            {
                return;
            }
            IAudioService audio = audioService;
            var context = new MainMenuViewContext(levels, navigation, playSystem, audio);
            navigation.Open(context);
        }
    }
}
