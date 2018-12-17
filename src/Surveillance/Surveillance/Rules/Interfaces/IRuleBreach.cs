using System;
using DomainV2.Financial;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Interfaces
{
    public interface IRuleBreach
    {
        TimeSpan Window { get; }
        ITradePosition Trades { get; }
        FinancialInstrument Security { get; }
        bool IsBackTestRun { get; set; }
    }
}
