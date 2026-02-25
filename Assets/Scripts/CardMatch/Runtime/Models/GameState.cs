using System.Collections.Generic;
using CardMatch.Config;

namespace CardMatch.CardMatch
{
    public class GameState
    {
        public LayoutConfig Layout { get; set; }
        public Dictionary<int, Card> Cards { get; set; }
        public List<Card> FlippedCards { get; set; }
        public int Score { get; set; }
        public int Round { get; set; }
        public int Combo { get; set; }

        public GameState()
        {
            Cards = new Dictionary<int, Card>();
            FlippedCards = new List<Card>();
        }

        public int SlotCount
        {
            get
            {
                if (Layout == null)
                {
                    return 0;
                }
                return Layout.Rows * Layout.Columns;
            }
        }
    }
}
