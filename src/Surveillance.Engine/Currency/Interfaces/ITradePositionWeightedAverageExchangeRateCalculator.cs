using System.Threading.Tasks;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Currency.Interfaces
{
    public interface ITradePositionWeightedAverageExchangeRateCalculator
    {
        Task<decimal> WeightedExchangeRate(
            ITradePosition position,
            Domain.Financial.Currency targetCurrency,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}