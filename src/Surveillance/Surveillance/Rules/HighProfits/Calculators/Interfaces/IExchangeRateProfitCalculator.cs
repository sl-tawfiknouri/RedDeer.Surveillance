using System.Threading.Tasks;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Interfaces
{
    public interface IExchangeRateProfitCalculator
    {
        Task<ExchangeRateProfitBreakdown> ExchangeRateMovement(
            ITradePosition positionCost,
            ITradePosition positionRevenue,
            Domain.Finance.Currency variableCurrency,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}