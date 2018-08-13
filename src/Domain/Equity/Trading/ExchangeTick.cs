using Domain.Equity.Market;
using System.Collections.Generic;

namespace Domain.Equity.Trading
{
    /// <summary>
    /// An aggregatino of security ticks
    /// </summary>
    public class ExchangeTick
    {
        /// <summary>
        /// The exchange the data update is issued by
        /// </summary>
        public StockExchange Exchange { get; }

        /// <summary>
        /// The securities with updated data
        /// </summary>
        IReadOnlyCollection<SecurityTick> Securities { get; }
    }
}
