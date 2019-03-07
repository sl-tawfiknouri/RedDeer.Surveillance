using System;
using System.Collections.Generic;
using Domain.Core.Markets.Timebars;

namespace Domain.Core.Markets.Collections
{
    /// <summary>
    /// An aggregation of security time bars
    /// This represents a specific point in time
    /// All child security dates are considered cannon at the same point in time
    /// If you have security data spanning multiple points in time they need
    /// to be split into multiple collections
    /// </summary>
    public class EquityIntraDayTimeBarCollection
    {
        public EquityIntraDayTimeBarCollection(
            Market exchange,
            DateTime epoch,
            IReadOnlyCollection<EquityInstrumentIntraDayTimeBar> securities)
        {
            Exchange = exchange;
            Epoch = epoch;
            Securities = securities ?? new List<EquityInstrumentIntraDayTimeBar>();
        }

        /// <summary>
        /// The exchange the data update is issued by
        /// </summary>
        public Market Exchange { get; }

        /// <summary>
        /// The securities with updated data
        /// </summary>
        public IReadOnlyCollection<EquityInstrumentIntraDayTimeBar> Securities { get; }

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
