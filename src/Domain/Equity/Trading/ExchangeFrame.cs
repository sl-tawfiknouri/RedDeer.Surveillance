using Domain.Equity.Market;
using System.Collections.Generic;

namespace Domain.Equity.Trading
{
    /// <summary>
    /// An aggregatino of security ticks
    /// </summary>
    public class ExchangeFrame
    {
        public ExchangeFrame(
            StockExchange exchange,
            IReadOnlyCollection<SecurityFrame> securities)
        {
            Exchange = exchange;
            Securities = securities ?? new List<SecurityFrame>();
        }

        /// <summary>
        /// The exchange the data update is issued by
        /// </summary>
        public StockExchange Exchange { get; }

        /// <summary>
        /// The securities with updated data
        /// </summary>
        public IReadOnlyCollection<SecurityFrame> Securities { get; }
    }
}
