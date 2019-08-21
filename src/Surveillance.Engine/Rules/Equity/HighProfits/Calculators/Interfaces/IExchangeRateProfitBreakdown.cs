namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces
{
    using Domain.Core.Financial.Money;

    using Surveillance.Engine.Rules.Trades.Interfaces;

    public interface IExchangeRateProfitBreakdown
    {
        Currency FixedCurrency { get; }

        ITradePosition PositionCost { get; }

        decimal PositionCostWer { get; }

        ITradePosition PositionRevenue { get; }

        decimal PositionRevenueWer { get; }

        Currency VariableCurrency { get; }

        decimal AbsoluteAmountDueToWer();

        decimal RelativePercentageDueToWer();
    }
}