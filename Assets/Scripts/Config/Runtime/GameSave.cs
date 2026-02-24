namespace CardMatch.Config
{
    public record GameSave(int Rows, int Columns, int[] PairIds, bool[] Matched, int Score, int Combo);
}
