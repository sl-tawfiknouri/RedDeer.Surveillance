using System.Collections.Generic;
using Domain.Market;

namespace Domain.Equity.Frames
{
    /// <summary>
    /// An aggregation of security frames
    /// This represents a specific point in time
    /// All child security dates are considered cannon at the same point in time
    /// If you have security data spanning multiple points in time they need
    /// to be split into multiple frames
    /// </summary>
    public class ExchangeFrame
    {
        public ExchangeFrame(
            StockExchange exchange,
            IReadOnlyCollection<SecurityTick> securities)
        {
            Exchange = exchange;
            Securities = securities ?? new List<SecurityTick>();
        }

        /// <summary>
        /// The exchange the data update is issued by
        /// </summary>
        public StockExchange Exchange { get; }

        /// <summary>
        /// The securities with updated data
        /// </summary>
        public IReadOnlyCollection<SecurityTick> Securities { get; }

        public override string ToString()
        {
            var str = string.Empty;

            if (Exchange != null)
            {
                str += $"Exchange ({Exchange.Id.Id}.{Exchange.Name}) ";   
            }

            if (Securities != null)
            {
                str += $"Securities({Securities.Count})  ";
            }

            return str;
        }
    }
}
