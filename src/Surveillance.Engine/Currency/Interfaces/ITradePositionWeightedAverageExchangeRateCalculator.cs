using System.Threading.Tasks;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Currency.Interfaces
{
    public interface ITradePositionWeightedAverageExchangeRateCalculator
    {
        Task<decimal> WeightedExchangeRate(
            ITradePosition position,
            DomainV2.Financial.Currency targetCurrency,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}