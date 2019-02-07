using System;
using DomainV2.Financial;

namespace DomainV2.Equity.TimeBars
{
    /// <summary>
    /// Market update for financial instrument trading data
    /// </summary>
    public class EquityInstrumentInterDayTimeBar
    {
        public EquityInstrumentInterDayTimeBar(
            FinancialInstrument security,
            DailySummaryTimeBar dailySummaryTimeBar,
            DateTime timeStamp,
            Market market)
        {
            Security = security;
            TimeStamp = timeStamp;
            Market = market;
            DailySummaryTimeBar = dailySummaryTimeBar;
        }

        /// <summary>
        /// The security the tick data was related to
        /// </summary>
        public FinancialInstrument Security { get; }

        /// <summary>
        /// Daily summary of data about the financial instrument
        /// </summary>
        public DailySummaryTimeBar DailySummaryTimeBar { get; }

        /// <summary>
        /// The time point at which the data was canonical
        /// </summary>
        public DateTime TimeStamp { get; }

        /// <summary>
        /// The market the security is traded on
        /// </summary>
        public Market Market { get; }
    }
}