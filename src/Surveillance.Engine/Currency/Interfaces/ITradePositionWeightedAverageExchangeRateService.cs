using System.Threading.Tasks;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Currency.Interfaces
{
    public interface ITradePositionWeightedAverageExchangeRateService
    {
        Task<decimal> WeightedExchangeRate(
            ITradePosition position,
            Domain.Core.Financial.Money.Currency targetCurrency,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}