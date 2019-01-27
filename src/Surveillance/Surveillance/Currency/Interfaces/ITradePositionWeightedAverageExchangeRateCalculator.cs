using System;
using System.Threading.Tasks;
using Surveillance.Systems.Auditing.Context.Interfaces;
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