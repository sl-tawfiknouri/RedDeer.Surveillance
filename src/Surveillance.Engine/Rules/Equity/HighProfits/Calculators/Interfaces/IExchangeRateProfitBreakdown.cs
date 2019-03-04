using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces
{
    public interface IExchangeRateProfitBreakdown
    {
        ITradePosition PositionCost { get; }
        decimal PositionCostWer { get; }
        ITradePosition PositionRevenue { get; }
        decimal PositionRevenueWer { get; }

        Domain.Core.Financial.Currency FixedCurrency { get; }
        Domain.Core.Financial.Currency VariableCurrency { get; }

        decimal AbsoluteAmountDueToWer();
        decimal RelativePercentageDueToWer();
    }
}