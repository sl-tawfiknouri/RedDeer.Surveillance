using System;
using System.Collections.Generic;

namespace DomainV2.Equity.Frames
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
            DomainV2.Financial.Market exchange,
            DateTime timeStamp,
            IReadOnlyCollection<SecurityTick> securities)
        {
            Exchange = exchange;
            TimeStamp = timeStamp;
            Securities = securities ?? new List<SecurityTick>();
        }

        /// <summary>
        /// The exchange the data update is issued by
        /// </summary>
        public DomainV2.Financial.Market Exchange { get; }

        /// <summary>
        /// The securities with updated data
        /// </summary>
        public IReadOnlyCollection<SecurityTick> Securities { get; }

        public DateTime TimeStamp { get; }

        public override string ToString()
        {
            var str = string.Empty;

            if (Exchange != null)
            {
                str += $"Exchange ({Exchange.MarketIdentifierCode}.{Exchange.Name}) ";   
            }

            if (Securities != null)
            {
                str += $"Securities({Securities.Count})  ";
            }

            return str;
        }
    }
}
