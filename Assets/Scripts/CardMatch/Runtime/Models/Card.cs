namespace CardMatch.CardMatch
{
    public class Card
    {
        public int CardId { get; }
        public CardState State { get; set; }

        public Card(int cardId, CardState state = CardState.Hidden)
        {
            CardId = cardId;
            State = state;
        }
    }
}
