﻿using Domain.Core.Markets.Timebars;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Core.Markets.Collections
{
    /// <summary>
    ///     An aggregation of security time bars
    ///     This represents a specific point in time
    ///     All child security dates are considered cannon at the same point in time
    ///     If you have security data spanning multiple points in time they need
    ///     to be split into multiple collections
    /// </summary>
    public class FixedIncomeIntraDayTimeBarCollection
    {
        public FixedIncomeIntraDayTimeBarCollection(
            Market exchange,
            DateTime epoch,
            IReadOnlyCollection<FixedIncomeInstrumentIntraDayTimeBar> securities)
        {
            this.Exchange = exchange;
            this.Epoch = epoch;
            this.Securities = securities ?? new List<FixedIncomeInstrumentIntraDayTimeBar>();
        }

        public DateTime Epoch { get; }

        /// <summary>
        ///     The exchange the data update is issued by
        /// </summary>
        public Market Exchange { get; }

        /// <summary>
        ///     The securities with updated data
        /// </summary>
        public IReadOnlyCollection<FixedIncomeInstrumentIntraDayTimeBar> Securities { get; }

        public override string ToString()
        {
            var str = string.Empty;

            if (this.Exchange != null) str += $"Exchange ({this.Exchange.MarketIdentifierCode}.{this.Exchange.Name}) ";

            if (this.Securities != null) str += $"Securities({this.Securities.Count})";

            return str;
        }
    }
}
