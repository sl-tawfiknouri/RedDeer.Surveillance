using System;
using System.Linq;
using System.Threading.Tasks;
using Surveillance.Currency.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators
{
    public class ExchangeRateProfitCalculator : IExchangeRateProfitCalculator
    {
        private readonly ITradePositionWeightedAverageExchangeRateCalculator _werExchangeRateCalculator;

        public ExchangeRateProfitCalculator(ITradePositionWeightedAverageExchangeRateCalculator werExchangeRateCalculator)
        {
            _werExchangeRateCalculator =
                werExchangeRateCalculator
                ?? throw new ArgumentNullException(nameof(werExchangeRateCalculator));
        }

        public async Task<ExchangeRateProfitBreakdown> ExchangeRateMovement(
            ITradePosition positionCost,
            ITradePosition positionRevenue,
            Domain.Finance.Currency variableCurrency, 
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            var orderCurrency = positionCost.Get().FirstOrDefault(pos => !string.IsNullOrWhiteSpace(pos.OrderCurrency))?.OrderCurrency;

            if (string.IsNullOrWhiteSpace(orderCurrency))
            {
                orderCurrency =
                    positionRevenue
                        .Get()
                        .FirstOrDefault(pos => !string.IsNullOrWhiteSpace(pos.OrderCurrency))
                        ?.OrderCurrency;
            }

            var costRates = await _werExchangeRateCalculator.WeightedExchangeRate(positionCost, variableCurrency, ruleCtx);
            var revenueRates =  await _werExchangeRateCalculator.WeightedExchangeRate(positionRevenue, variableCurrency, ruleCtx);

            return new ExchangeRateProfitBreakdown(
                positionCost,
                positionRevenue,
                costRates,
                revenueRates,
                orderCurrency,
                variableCurrency.Value);
        }
    }
}
