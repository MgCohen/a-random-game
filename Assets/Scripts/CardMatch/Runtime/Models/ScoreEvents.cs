namespace CardMatch.CardMatch
{
    public record ScoreChanged(int Score, int AddedPoints) : MatchEvent;
    public record ComboChanged(int Combo) : MatchEvent;
}
