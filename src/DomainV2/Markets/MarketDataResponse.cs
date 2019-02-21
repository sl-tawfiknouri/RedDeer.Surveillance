namespace Domain.Markets
{
    public class MarketDataResponse<T> where T : class
    {
        public MarketDataResponse(T response, bool hadMissingData, bool isBestEffort)
        {
            Response = response;
            HadMissingData = hadMissingData;
        }

        public T Response { get; }
        public bool HadMissingData { get; }
        public bool IsBestEffort { get; }

        public static MarketDataResponse<T> MissingData()
        {
            return new MarketDataResponse<T>(null, true, false);
        }
    }
}
