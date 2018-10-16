using System;
using Domain.Market.Interfaces;

namespace Domain.Market
{
    /// <summary>
    /// Shared type for all mediums where equity securities are trading
    /// </summary>
    public abstract class Market : IMarket
    {
        protected Market(MarketId id, string name)
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

            public override int GetHashCode()
            {
                return Id?.GetHashCode() ?? 0;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is MarketId))
                {
                    return false;
                }

                var otherId = (MarketId)obj;
                return string.Equals(Id, otherId?.Id, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
