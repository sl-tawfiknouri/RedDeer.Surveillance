using System;
using DomainV2.Equity;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Interfaces
{
    public interface IRuleBreach
    {
        TimeSpan Window { get; }
        ITradePosition Trades { get; }
        Security Security { get; }
    }
}
