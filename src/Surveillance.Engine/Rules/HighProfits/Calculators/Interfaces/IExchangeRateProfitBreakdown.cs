using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Interfaces
{
    public interface IExchangeRateProfitBreakdown
    {
        ITradePosition PositionCost { get; }
        decimal PositionCostWer { get; }
        ITradePosition PositionRevenue { get; }
        decimal PositionRevenueWer { get; }

        DomainV2.Financial.Currency FixedCurrency { get; }
        DomainV2.Financial.Currency VariableCurrency { get; }

        decimal AbsoluteAmountDueToWer();
        decimal RelativePercentageDueToWer();
    }
}