using System;
using System.Collections.Generic;

namespace Domain.Equity.TimeBars
{
    /// <summary>
    /// An aggregation of security time bars
    /// This represents a specific point in time
    /// All child security dates are considered cannon at the same point in time
    /// If you have security data spanning multiple points in time they need
    /// to be split into multiple collections
    /// </summary>
    public class EquityInterDayTimeBarCollection
    {
        public EquityInterDayTimeBarCollection(
            Financial.Market exchange,
            DateTime epoch,
            IReadOnlyCollection<EquityInstrumentInterDayTimeBar> securities)
        {
            Exchange = exchange;
            Epoch = epoch;
            Securities = securities ?? new List<EquityInstrumentInterDayTimeBar>();
        }

        /// <summary>
        /// The exchange the data update is issued by
        /// </summary>
        public Financial.Market Exchange { get; }

        /// <summary>
        /// The securities with updated data
        /// </summary>
        public IReadOnlyCollection<EquityInstrumentInterDayTimeBar> Securities { get; }

        public DateTime Epoch { get; }

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
