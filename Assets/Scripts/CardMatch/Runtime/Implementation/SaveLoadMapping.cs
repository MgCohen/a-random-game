using System.Collections.Generic;
using CardMatch.Config;

namespace CardMatch.CardMatch
{
    public static class SaveLoadMapping
    {
        public static GameState ToState(GameSave save)
        {
            var layout = new LayoutConfig { Rows = save.Rows, Columns = save.Columns };
            int count = save.Rows * save.Columns;
            int round = CountRound(save.Matched);
            var state = new GameState { Layout = layout, Score = save.Score, Round = round, Combo = save.Combo };
            if (!HasValidArrays(save, count))
            {
                state.Cards = new Dictionary<int, Card>();
                state.FlippedCards = new List<Card>();
                return state;
            }
            state.Cards = BuildCards(save, count);
            state.FlippedCards = new List<Card>();
            return state;
        }

        public static GameSave ToSave(GameState state)
        {
            int count = state.SlotCount;
            var pairIds = new int[count];
            var matched = new bool[count];
            FillSaveArrays(state, pairIds, matched, count);
            return new GameSave(state.Layout.Rows, state.Layout.Columns, pairIds, matched, state.Score, state.Combo);
        }

        private static int CountRound(bool[] matched)
        {
            if (matched == null)
            {
                return 0;
            }
            int matchCount = 0;
            for (int i = 0; i < matched.Length; i++)
            {
                if (matched[i])
                {
                    matchCount += 1;
                }
            }
            return matchCount / 2;
        }

        private static bool HasValidArrays(GameSave save, int count)
        {
            if (save.PairIds == null || save.Matched == null)
            {
                return false;
            }
            if (save.PairIds.Length != count)
            {
                return false;
            }
            return save.Matched.Length == count;
        }

        private static Dictionary<int, Card> BuildCards(GameSave save, int count)
        {
            var cards = new Dictionary<int, Card>(count);
            for (int i = 0; i < count; i++)
            {
                CardState state = save.Matched[i] ? CardState.Scored : CardState.Hidden;
                cards[i] = new Card(save.PairIds[i], state);
            }
            return cards;
        }

        private static void FillSaveArrays(GameState state, int[] pairIds, bool[] matched, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (!state.Cards.TryGetValue(i, out Card card))
                {
                    continue;
                }
                pairIds[i] = card.CardId;
                matched[i] = card.State == CardState.Scored;
            }
        }
    }
}
