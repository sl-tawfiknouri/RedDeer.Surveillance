using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Currency
{
    public class TradePositionWeightedAverageExchangeRateService : ITradePositionWeightedAverageExchangeRateService
    {
        private readonly IExchangeRatesService _exchangeRatesService;
        private readonly ILogger<TradePositionWeightedAverageExchangeRateService> _logger;

        public TradePositionWeightedAverageExchangeRateService(
            IExchangeRatesService exchangeRatesService,
            ILogger<TradePositionWeightedAverageExchangeRateService> logger)
        {
            _exchangeRatesService = exchangeRatesService ?? throw new ArgumentNullException(nameof(exchangeRatesService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<decimal> WeightedExchangeRate(
            ITradePosition position,
            Domain.Core.Financial.Money.Currency targetCurrency,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            if (position == null
                || position.TotalVolume() == 0)
            {
                _logger.LogInformation($"asked to calculate WER for either null position or position with 0 volume. Returning 0");
                return 0;
            }

            var totalVolume = position.TotalVolume();
            var weightedRates = new List<WeightedXRate>();
            
            foreach (var order in position.Get())
            {
                if (order.OrderFilledVolume.GetValueOrDefault() == 0)
                    continue;

                var weight = (decimal)order.OrderFilledVolume.GetValueOrDefault(0) / (decimal)totalVolume;

                var rate = await _exchangeRatesService.GetRate(
                    order.OrderCurrency,
                    targetCurrency,
                    order.MostRecentDateEvent(),
                    ruleCtx);
                
                weightedRates.Add(new WeightedXRate(weight, ((decimal?)rate?.Rate ?? 0m)));
            }

            foreach (var item in weightedRates)
            {
                if (item == null)
                    continue;

                _logger.LogInformation($"had a sub component with {item.Weight} and {item.XRate}");
            }

            var weightedAverage = weightedRates.Sum(wr => wr.Weight * wr.XRate);

            return weightedAverage;
        }

        public class WeightedXRate
        {
            public WeightedXRate(decimal weight, decimal xRate)
            {
                Weight = weight;
                XRate = xRate;
            }

            public decimal Weight { get; }
            public decimal XRate { get; }
        }
    }
}
