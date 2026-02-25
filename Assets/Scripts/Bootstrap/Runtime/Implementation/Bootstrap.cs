using System;
using System.Collections.Generic;
using System.Reflection;
using CardMatch.CardMatch;
using CardMatch.Config;
using CardMatch.Navigation;
using UnityEngine;


namespace CardMatch.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private Level level;
        [SerializeField] private View[] viewPrefabs;
        [SerializeField] private Transform viewRoot;
        [SerializeField] private int[] shuffledCardIds;

        private INavigation navigation;
        private IMatchEvents matchEvents;
        private Match match;
        private ScoreService scoreService;
        private Type initialViewType;

        public INavigation Navigation
        {
            get
            {
                return navigation;
            }
        }

        public IMatchEvents MatchEvents
        {
            get
            {
                return matchEvents;
            }
        }

        public Match Match
        {
            get
            {
                return match;
            }
        }

        public ScoreService ScoreService
        {
            get
            {
                return scoreService;
            }
        }

        private void Start()
        {
            Build();
        }

        public void Build()
        {
            INavigation builtNavigation = BuildNavigation();
            BuildCardMatch();
            OpenInitialView(builtNavigation, initialViewType);
        }

        private INavigation BuildNavigation()
        {
            IView[] instantiatedViews = InstantiateViews();
            var builtNavigation = new NavigationController(instantiatedViews);
            initialViewType = GetInitialViewType(instantiatedViews);
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

        private void BuildCardMatch()
        {
            LevelConfig config = GetLevelConfig();
            GameState state = BuildGameState(config);
            var matchEventsService = new TypedEventService();
            Match builtMatch = new Match(state, matchEventsService);
            var builtScoreService = new ScoreService(state, config.Scoring, matchEventsService);
            matchEvents = matchEventsService;
            match = builtMatch;
            scoreService = builtScoreService;
        }

        private LevelConfig GetLevelConfig()
        {
            if (level != null && level.Config != null)
            {
                return level.Config;
            }
            return new LevelConfig();
        }

        private GameState BuildGameState(LevelConfig config)
        {
            var state = new GameState();
            state.Layout = config.Layout;
            state.Cards = BuildCards(config.Layout);
            state.FlippedCards = new List<Card>();
            state.Score = 0;
            state.Round = 0;
            state.Combo = 0;
            return state;
        }

        private Dictionary<int, Card> BuildCards(LayoutConfig layout)
        {
            int slotCount = GetSlotCount(layout);
            if (slotCount <= 0)
            {
                return new Dictionary<int, Card>();
            }
            if (shuffledCardIds == null || shuffledCardIds.Length != slotCount)
            {
                return new Dictionary<int, Card>();
            }
            var cards = new Dictionary<int, Card>(slotCount);
            for (int i = 0; i < slotCount; i++)
            {
                cards[i] = new Card(shuffledCardIds[i], CardState.Hidden);
            }
            return cards;
        }

        private static int GetSlotCount(LayoutConfig layout)
        {
            if (layout == null)
            {
                return 0;
            }
            return layout.Rows * layout.Columns;
        }

        private void OpenInitialView(INavigation builtNavigation, Type viewType)
        {
            if (builtNavigation == null || viewType == null)
            {
                return;
            }
            if (builtNavigation.CurrentView != null)
            {
                return;
            }
            MethodInfo openMethod = typeof(INavigation).GetMethod("Open");
            if (openMethod == null)
            {
                return;
            }
            MethodInfo genericMethod = openMethod.MakeGenericMethod(viewType);
            genericMethod.Invoke(builtNavigation, null);
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

        private void OnDestroy()
        {
            if (scoreService == null)
            {
                return;
            }
            scoreService.Dispose();
        }
    }
}
