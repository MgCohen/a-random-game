using System.Collections.Generic;
using CardMatch.Config;

namespace CardMatch.CardMatch
{
    public class GameState
    {
        public LayoutConfig Layout { get; set; }
        public List<Card> Cards { get; set; }
        public List<Card> FlippedCards { get; set; }
        public int Score { get; set; }
        public int Round { get; set; }
        public int Combo { get; set; }

        public GameState()
        {
            Cards = new List<Card>();
            FlippedCards = new List<Card>();
        }
    }
}
