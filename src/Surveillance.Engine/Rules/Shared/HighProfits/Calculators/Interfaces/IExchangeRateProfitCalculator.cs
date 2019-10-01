using System.Threading.Tasks;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces
{
    using Domain.Core.Trading.Interfaces;

    public interface IExchangeRateProfitCalculator
    {
        Task<ExchangeRateProfitBreakdown> ExchangeRateMovement(
            ITradePosition positionCost,
            ITradePosition positionRevenue,
            Domain.Core.Financial.Money.Currency variableCurrency,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}