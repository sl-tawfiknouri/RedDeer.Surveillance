using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Interfaces
{
    public interface IExchangeRateProfitBreakdown
    {
        ITradePosition PositionCost { get; }
        decimal PositionCostWer { get; }
        ITradePosition PositionRevenue { get; }
        decimal PositionRevenueWer { get; }

        string FixedCurrency { get; }
        string VariableCurrency { get; }

        decimal AbsoluteAmountDueToWer();
        decimal RelativePercentageDueToWer();
    }
}