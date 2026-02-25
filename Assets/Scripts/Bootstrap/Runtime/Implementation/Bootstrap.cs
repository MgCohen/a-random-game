using System;
using System.Collections.Generic;
using System.Reflection;
using CardMatch.CardMatch;
using CardMatch.Levels;
using CardMatch.MainMenu;
using CardMatch.Navigation;
using CardMatch.Persistence;
using UnityEngine;
using UnityEngine.Serialization;

namespace CardMatch.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private LevelRegistry levelRegistry;
        [SerializeField] private View[] views;

        private IPersistence persistence;
        private INavigation navigation;
        private ILevelController levels;

        private void Start()
        {
            Build();
        }

        public void Build()
        {
            persistence = new PlayerPrefsPersistence();
            navigation = new NavigationController(views);
            levels = new LevelController(levelRegistry, persistence);
            OpenInitialView();
        }

        private void OpenInitialView()
        {
            if (navigation == null || levelRegistry == null)
            {
                return;
            }
            var context = new MainMenuViewContext(levels, navigation);
            navigation.Open(context);
        }
    }
}
