using System.Threading.Tasks;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Interfaces
{
    public interface IExchangeRateProfitCalculator
    {
        Task<ExchangeRateProfitBreakdown> ExchangeRateMovement(
            ITradePosition positionCost,
            ITradePosition positionRevenue,
            DomainV2.Financial.Currency variableCurrency,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}