using Domain.Core.Financial.Assets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Core.Markets.Timebars
{
    /// <summary>
    ///     Market update for financial instrument trading data
    /// </summary>
    public class FixedIncomeInstrumentInterDayTimeBar
    {
        public FixedIncomeInstrumentInterDayTimeBar(
            FinancialInstrument security,
            DailySummaryTimeBar dailySummaryTimeBar,
            DateTime timeStamp,
            Market market)
        {
            this.Security = security;
            this.TimeStamp = timeStamp;
            this.Market = market;
            this.DailySummaryTimeBar = dailySummaryTimeBar;
        }

        /// <summary>
        ///     Daily summary of data about the financial instrument
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
        ///     The time point at which the data was canonical
        /// </summary>
        public DateTime TimeStamp { get; }
    }
}
