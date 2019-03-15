using Domain.Core.Trading.Interfaces;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

// ReSharper disable UnusedMember.Global

namespace Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces
{
    public interface ISpoofingRuleBreach : IRuleBreach
    {
        Order MostRecentTrade { get; }
        ITradePosition TradesInFulfilledPosition { get; }
        ITradePosition CancelledTrades { get; }
    }
}