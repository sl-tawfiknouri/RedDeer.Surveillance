using System.Threading.Tasks;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces
{
    public interface IExchangeRateProfitCalculator
    {
        Task<ExchangeRateProfitBreakdown> ExchangeRateMovement(
            ITradePosition positionCost,
            ITradePosition positionRevenue,
            Domain.Core.Financial.Currency variableCurrency,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}