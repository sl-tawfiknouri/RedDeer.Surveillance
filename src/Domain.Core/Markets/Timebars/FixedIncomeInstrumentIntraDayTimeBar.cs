using Domain.Core.Financial.Assets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Core.Markets.Timebars
{
    /// <summary>
    ///     Intraday update for financial instrument trading data
    /// </summary>
    public class FixedIncomeInstrumentIntraDayTimeBar
    {
        public FixedIncomeInstrumentIntraDayTimeBar(
            FinancialInstrument security,
            SpreadTimeBar spreadTimeBar,
            DailySummaryTimeBar dailySummaryTimeBar,
            DateTime timeStamp,
            Market market)
        {
            this.Security = security;
            this.SpreadTimeBar = spreadTimeBar;
            this.TimeStamp = timeStamp;
            this.Market = market;
            this.DailySummaryTimeBar = dailySummaryTimeBar;
        }

        /// <summary>
        ///     Daily summary of data about the financial instrument;
        ///     this is provided in addition to the spread time bar.
        ///     Its cannon representation
        /// </summary>
        public DailySummaryTimeBar DailySummaryTimeBar { get; }

        /// <summary>
        ///     The market the security is traded on
        /// </summary>
        public Market Market { get; }

        /// <summary>
        ///     The security the tick data was related to
        /// </summary>
        public FinancialInstrument Security { get; }

        /// <summary>
        ///     Price spread at the tick point
        /// </summary>
        public SpreadTimeBar SpreadTimeBar { get; }

        /// <summary>
        ///     The time point at which the data was canonical
        /// </summary>
        public DateTime TimeStamp { get; }
    }
}
