using System;
using System.Collections.Generic;
using System.Reflection;
using CardMatch.CardMatch;
using CardMatch.Levels;
using CardMatch.MainMenu;
using CardMatch.Navigation;
using CardMatch.Persistence;
using UnityEngine;

namespace CardMatch.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private LevelRegistry levelRegistry;
        [SerializeField] private View[] viewPrefabs;
        [SerializeField] private Transform viewRoot;

        private IPersistence persistence;
        private INavigation navigation;

        public INavigation Navigation
        {
            get
            {
                return navigation;
            }
        }

        private void Start()
        {
            Build();
        }

        public void Build()
        {
            INavigation builtNavigation = BuildNavigation();
            OpenInitialView();
        }

        private INavigation BuildNavigation()
        {
            IView[] instantiatedViews = InstantiateViews();
            var builtNavigation = new NavigationController(instantiatedViews);
            BindNavigationToViews(builtNavigation, instantiatedViews);
            InitializeViews(instantiatedViews);
            navigation = builtNavigation;
            return builtNavigation;
        }

        private IView[] InstantiateViews()
        {
            if (viewPrefabs == null || viewPrefabs.Length == 0)
            {
                return Array.Empty<IView>();
            }
            var instantiatedViews = new List<IView>(viewPrefabs.Length);
            for (int i = 0; i < viewPrefabs.Length; i++)
            {
                View prefab = viewPrefabs[i];
                if (prefab == null)
                {
                    continue;
                }
                View instance = Instantiate(prefab, viewRoot);
                instantiatedViews.Add(instance);
            }
            return instantiatedViews.ToArray();
        }

        private static void BindNavigationToViews(INavigation builtNavigation, IView[] instantiatedViews)
        {
            for (int i = 0; i < instantiatedViews.Length; i++)
            {
                IView view = instantiatedViews[i];
                if (view is not View concreteView)
                {
                    continue;
                }
                concreteView.SetNavigation(builtNavigation);
            }
        }

        private static void InitializeViews(IView[] instantiatedViews)
        {
            for (int i = 0; i < instantiatedViews.Length; i++)
            {
                IView view = instantiatedViews[i];
                if (view is not IInitializable initializable)
                {
                    continue;
                }
                initializable.Initialize();
            }
        }

        private void OpenInitialView()
        {
            if (navigation == null || levelRegistry == null)
            {
                return;
            }
            ILevelController levelController = new LevelController(levelRegistry, persistence);
            var context = new MainMenuViewContext(levelController, navigation);
            navigation.Open(context);
        }

        private static Type GetInitialViewType(IView[] views)
        {
            if (views == null || views.Length == 0)
            {
                return null;
            }
            IView firstView = views[0];
            if (firstView == null)
            {
                return null;
            }
            return firstView.GetType();
        }
    }
}
