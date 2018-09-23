using System;
using Domain.Equity;
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
