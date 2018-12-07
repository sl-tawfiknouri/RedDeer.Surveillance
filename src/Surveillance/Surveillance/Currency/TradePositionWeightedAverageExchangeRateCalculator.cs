﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Surveillance.Currency.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Currency
{
    public class TradePositionWeightedAverageExchangeRateCalculator : ITradePositionWeightedAverageExchangeRateCalculator
    {
        private readonly IExchangeRates _exchangeRates;
        
        public TradePositionWeightedAverageExchangeRateCalculator(IExchangeRates exchangeRates)
        {
            _exchangeRates = exchangeRates ?? throw new ArgumentNullException(nameof(exchangeRates));
        }

        public async Task<decimal> WeightedExchangeRate(
            ITradePosition position,
            Domain.Finance.Currency targetCurrency,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            if (position == null
                || position.TotalVolume() == 0)
            {
                return 0;
            }

            var totalVolume = position.TotalVolume();
            var weightedRates = new List<WeightedXRate>();
            
            foreach (var trad in position.Get())
            {
                if (trad.FulfilledVolume == 0)
                    continue;

                var weight = (decimal)trad.FulfilledVolume / (decimal)totalVolume;

                var orderCurrency = new Domain.Finance.Currency(trad.OrderCurrency);
                var rate = await _exchangeRates.GetRate(
                    orderCurrency,
                    targetCurrency,
                    trad.StatusChangedOn,
                    ruleCtx);
                
                weightedRates.Add(new WeightedXRate(weight, ((decimal?)rate?.Rate ?? 0m)));
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