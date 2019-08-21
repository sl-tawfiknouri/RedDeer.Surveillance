// ReSharper disable UnusedMember.Global

namespace Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces
{
    using Domain.Core.Trading.Orders;

    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades.Interfaces;

    public interface ISpoofingRuleBreach : IRuleBreach
    {
        ITradePosition CancelledTrades { get; }

        Order MostRecentTrade { get; }

        ITradePosition TradesInFulfilledPosition { get; }
    }
}