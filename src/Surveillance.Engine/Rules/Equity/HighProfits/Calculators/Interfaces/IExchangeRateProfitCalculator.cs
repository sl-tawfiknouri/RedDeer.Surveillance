namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Core.Financial.Money;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Trades.Interfaces;

    public interface IExchangeRateProfitCalculator
    {
        Task<ExchangeRateProfitBreakdown> ExchangeRateMovement(
            ITradePosition positionCost,
            ITradePosition positionRevenue,
            Currency variableCurrency,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}