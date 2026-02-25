namespace CardMatch.CardMatch
{
    public record CardStateChanged(Card Card, CardState State) : MatchEvent;
    public record CardsMatched(Card First, Card Second) : MatchEvent;
    public record CardsMismatched(Card First, Card Second) : MatchEvent;
    public record MatchCompleted(GameState FinalState) : MatchEvent;
}
