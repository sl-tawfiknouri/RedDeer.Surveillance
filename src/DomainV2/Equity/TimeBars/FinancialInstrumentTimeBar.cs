﻿using System;
using DomainV2.Financial;

namespace DomainV2.Equity.TimeBars
{
    /// <summary>
    /// Market update for financial instrument trading data
    /// </summary>
    public class FinancialInstrumentTimeBar
    {
        public FinancialInstrumentTimeBar(
            FinancialInstrument security,
            SpreadTimeBar spreadTimeBar,
            DailySummaryTimeBar dailySummaryTimeBar,
            DateTime timeStamp,
            Market market)
        {
            Security = security;
            SpreadTimeBar = spreadTimeBar;
            TimeStamp = timeStamp;
            Market = market;
            DailySummaryTimeBar = dailySummaryTimeBar;
        }

        /// <summary>
        /// The security the tick data was related to
        /// </summary>
        public FinancialInstrument Security { get; }

        /// <summary>
        /// Price spread at the tick point
        /// </summary>
        public SpreadTimeBar SpreadTimeBar { get; }

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