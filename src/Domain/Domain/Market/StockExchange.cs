using System;

namespace Domain.Market
{
    /// <summary>
    /// Stock exchanges LSE
    /// </summary>
    public class StockExchange : Market
    {
        public StockExchange(MarketId id, string name) 
            : base(id, name)
        {
        }

        public override int GetHashCode()
        {
            return Id?.Id?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            return obj is StockExchange otherExchange
                   && string.Equals(otherExchange.Id?.Id, Id?.Id, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}