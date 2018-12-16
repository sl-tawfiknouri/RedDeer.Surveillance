namespace Surveillance.Markets
{
    public class MarketDataResponse<T> where T : class
    {
        public MarketDataResponse(T response, bool hadMissingData)
        {
            Response = response;
            HadMissingData = hadMissingData;
        }

        public T Response { get; }
        public bool HadMissingData { get; }

        public static MarketDataResponse<T> MissingData()
        {
            return new MarketDataResponse<T>(null, true);
        }
    }
}
