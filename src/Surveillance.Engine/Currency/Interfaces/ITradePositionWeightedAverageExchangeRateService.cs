namespace Surveillance.Engine.Rules.Currency.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Core.Financial.Money;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Trades.Interfaces;

    public interface ITradePositionWeightedAverageExchangeRateService
    {
        Task<decimal> WeightedExchangeRate(
            ITradePosition position,
            Currency targetCurrency,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}