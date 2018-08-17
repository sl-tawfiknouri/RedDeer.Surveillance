namespace Domain.Market
{
    /// <summary>
    /// Shared type for all mediums where equity securities are trading
    /// </summary>
    public abstract class Market
    {
        public Market(MarketId id, string name)
        {
            Id = id;
            Name = name ?? string.Empty;
        }

        public MarketId Id { get; }

        public string Name { get; }

        public class MarketId
        {
            public MarketId(string id)
            {
                Id = id ?? string.Empty;
            }

            public string Id { get; }
        }
    }
}
