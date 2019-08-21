namespace SharedKernel.Contracts.Markets
{
    public class MarketDataResponse<T>
        where T : class
    {
        public MarketDataResponse(T response, bool hadMissingData, bool isBestEffort)
        {
            this.Response = response;
            this.HadMissingData = hadMissingData;
        }

        public bool HadMissingData { get; }

        public bool IsBestEffort { get; }

        public T Response { get; }

        public static MarketDataResponse<T> MissingData()
        {
            return new MarketDataResponse<T>(null, true, false);
        }
    }
}