namespace CardMatch.Persistence
{
    public interface IPersistence
    {
        void Save<T>(T value, string key = default);
        T Load<T>(string key = default);
        void ClearAll();
    }
}
