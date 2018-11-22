using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators
{
    public class TradePositionWeightedAverageExchangeRateCalculator
    {
        private readonly IExchangeRateApiCachingDecoratorRepository _exchangeRateApiRepository;

        public TradePositionWeightedAverageExchangeRateCalculator(IExchangeRateApiCachingDecoratorRepository exchangeRateApiRepository)
        {
            _exchangeRateApiRepository = exchangeRateApiRepository ?? throw new ArgumentNullException(nameof(exchangeRateApiRepository));
        }

        public async Task<decimal> WeightedExchangeRate(ITradePosition position, DateTime universeTime)
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
                var ratesOnDay = await _exchangeRateApiRepository.Get(trad.StatusChangedOn.Date, trad.StatusChangedOn.Date);
                
                weightedRates.Add(new WeightedXRate(weight, 0));
            }

            if (weightedRates.Sum(wr => wr.Weight) != 1)
            {
                throw new InvalidOperationException("summed weighted averages didn't add up to one");
            }
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
