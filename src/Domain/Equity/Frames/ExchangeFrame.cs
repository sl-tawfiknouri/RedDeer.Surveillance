using Domain.Market;
using System.Collections.Generic;

namespace Domain.Equity.Trading.Frames
{
    /// <summary>
    /// An aggregation of security frames
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

        public override string ToString()
        {
            var str = string.Empty;

            if (Exchange != null)
            {
                str += $"|Exchange.{Exchange.Id}.{Exchange.Name}";   
            }

            if (Securities != null)
            {
                str += $"|Securities.{Securities.Count}";
            }

            return str;
        }
    }
}
