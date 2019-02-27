using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators
{
    public class ExchangeRateProfitCalculator : IExchangeRateProfitCalculator
    {
        private readonly ITradePositionWeightedAverageExchangeRateCalculator _werExchangeRateCalculator;
        private readonly ILogger<ExchangeRateProfitCalculator> _logger;

        public ExchangeRateProfitCalculator(
            ITradePositionWeightedAverageExchangeRateCalculator werExchangeRateCalculator,
            ILogger<ExchangeRateProfitCalculator> logger)
        {
            _werExchangeRateCalculator =
                werExchangeRateCalculator
                ?? throw new ArgumentNullException(nameof(werExchangeRateCalculator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ExchangeRateProfitBreakdown> ExchangeRateMovement(
            ITradePosition positionCost,
            ITradePosition positionRevenue,
            Domain.Financial.Currency variableCurrency, 
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            if (string.IsNullOrEmpty(variableCurrency.Value))
            {
                _logger.LogInformation($"ExchangeRateProfitCalculator ExchangeRateMovement had a null or empty variable currency. Returning null.");
                return null;
            }

            var orderCurrency = positionCost.Get().FirstOrDefault(pos => !string.IsNullOrWhiteSpace(pos.OrderCurrency.Value))?.OrderCurrency;

            if (string.Equals(orderCurrency?.Value, variableCurrency.Value, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogInformation($"ExchangeRateProfitCalculator ExchangeRateMovement could not find an order currency. Returning null.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(orderCurrency.GetValueOrDefault().Value))
            {
                orderCurrency =
                    positionRevenue
                        .Get()
                        .FirstOrDefault(pos => !string.IsNullOrWhiteSpace(pos.OrderCurrency.Value))
                        ?.OrderCurrency;
            }

            var costRates = await _werExchangeRateCalculator.WeightedExchangeRate(positionCost, variableCurrency, ruleCtx);
            var revenueRates =  await _werExchangeRateCalculator.WeightedExchangeRate(positionRevenue, variableCurrency, ruleCtx);

            var breakdown = new ExchangeRateProfitBreakdown(
                positionCost,
                positionRevenue,
                costRates,
                revenueRates,
                orderCurrency.GetValueOrDefault(),
                variableCurrency);

            return breakdown;
        }
    }
}
