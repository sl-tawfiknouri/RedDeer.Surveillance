using System.Threading.Tasks;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Currency.Interfaces
{
    public interface ITradePositionWeightedAverageExchangeRateCalculator
    {
        Task<decimal> WeightedExchangeRate(
            ITradePosition position,
            DomainV2.Financial.Currency targetCurrency,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}