using System;
using System.Collections.Generic;
using CardMatch.CardMatch;

namespace CardMatch.PlaySystem
{
    public class PlayMatchFactory : IPlayMatchFactory
    {
        public Match Create(Level level)
        {
            Match match = null;
            bool canCreate = level != null;
            if (canCreate) match = CreateMatch(level);
            return match;
        }

        private Match CreateMatch(Level level)
        {
            GameState state = BuildState(level);
            ScoreRules scoreRules = level.Config.Scoring;
            Match match = new Match(state, scoreRules);
            return match;
        }

        private GameState BuildState(Level level)
        {
            LayoutConfig layout = level.Config.Layout;
            List<Card> cards = BuildShuffledCards(layout);
            var state = new GameState();
            state.Layout = layout;
            state.Cards = cards;
            ResetStateFields(state);
            return state;
        }

        private void ResetStateFields(GameState state)
        {
            state.FlippedCards = new List<Card>();
            state.Score = 0;
            state.Round = 0;
            state.Combo = 0;
        }

        private List<Card> BuildShuffledCards(LayoutConfig layout)
        {
            int slotCount = GetSlotCount(layout);
            bool hasValidSlotCount = IsEvenPositive(slotCount);
            List<Card> cards = new List<Card>();
            if (hasValidSlotCount) cards = BuildShuffledPairs(slotCount);
            return cards;
        }

        private List<Card> BuildShuffledPairs(int slotCount)
        {
            int pairCount = slotCount / 2;
            List<int> cardIds = BuildCardIds(pairCount);
            Shuffle(cardIds);
            List<Card> cards = BuildCards(cardIds);
            return cards;
        }

        private int GetSlotCount(LayoutConfig layout)
        {
            int rows = GetRows(layout);
            int columns = GetColumns(layout);
            return rows * columns;
        }

        private int GetRows(LayoutConfig layout)
        {
            if (layout == null) return 0;
            return layout.Rows;
        }

        private int GetColumns(LayoutConfig layout)
        {
            if (layout == null) return 0;
            return layout.Columns;
        }

        private bool IsEvenPositive(int slotCount)
        {
            if (slotCount <= 0) return false;
            return slotCount % 2 == 0;
        }

        private List<int> BuildCardIds(int pairCount)
        {
            var cardIds = new List<int>();
            for (int id = 1; id <= pairCount; id++)
            {
                AddPair(cardIds, id);
            }
            return cardIds;
        }

        private void AddPair(IList<int> ids, int cardId)
        {
            ids.Add(cardId);
            ids.Add(cardId);
        }

        private void Shuffle(IList<int> list)
        {
            int seed = Environment.TickCount;
            var random = new Random(seed);
            for (int i = list.Count - 1; i > 0; i--)
            {
                Swap(list, i, random.Next(i + 1));
            }
        }

        private void Swap(IList<int> list, int left, int right)
        {
            int temp = list[left];
            list[left] = list[right];
            list[right] = temp;
        }

        private List<Card> BuildCards(IList<int> ids)
        {
            var cards = new List<Card>(ids.Count);
            for (int i = 0; i < ids.Count; i++)
            {
                cards.Add(CreateCard(ids[i]));
            }
            return cards;
        }

        private Card CreateCard(int id)
        {
            Card card = new Card(id, CardState.Hidden);
            return card;
        }
    }
}
