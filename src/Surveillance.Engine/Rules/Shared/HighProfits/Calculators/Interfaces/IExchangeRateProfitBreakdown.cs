using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces
{
    using Domain.Core.Trading.Interfaces;

    public interface IExchangeRateProfitBreakdown
    {
        Domain.Core.Financial.Money.Currency FixedCurrency { get; }

        ITradePosition PositionCost { get; }

        decimal PositionCostWer { get; }

        ITradePosition PositionRevenue { get; }

        decimal PositionRevenueWer { get; }

        Domain.Core.Financial.Money.Currency VariableCurrency { get; }

        decimal AbsoluteAmountDueToWer();

        decimal RelativePercentageDueToWer();
    }
}