using Domain.Trading;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

// ReSharper disable UnusedMember.Global

namespace Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces
{
    public interface ISpoofingRuleBreach : IRuleBreach
    {
        ITradePosition TradesInFulfilledPosition { get; }
        ITradePosition CancelledTrades { get; }
        Order MostRecentTrade { get; }
    }
}