using System.Threading.Tasks;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Interfaces
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