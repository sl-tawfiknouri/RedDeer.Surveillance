using System;
using System.Linq;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Markets
{
    public class MarketTradingHoursManager : IMarketTradingHoursManager
    {
        private readonly IMarketOpenCloseApiCachingDecoratorRepository _repository;

        public MarketTradingHoursManager(IMarketOpenCloseApiCachingDecoratorRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ITradingHours Get(string marketIdentifierCode)
        {
            if (string.IsNullOrWhiteSpace(marketIdentifierCode))
            {
                return new TradingHours { Mic = marketIdentifierCode, IsValid = false };
            }

            var resultTask = _repository.Get();
            resultTask.Wait();
            var result = resultTask.Result;

            var exchange = result.FirstOrDefault(res => string.Equals(res.Code, marketIdentifierCode, StringComparison.InvariantCultureIgnoreCase));

            if (exchange == null)
            {
                return new TradingHours { Mic = marketIdentifierCode, IsValid = false };
            }

            return new TradingHours
            {
                Mic = marketIdentifierCode,
                IsValid = true,
                OpenOffsetInUtc = exchange.MarketOpenTime,
                CloseOffsetInUtc = exchange.MarketCloseTime
            };
        }
    }
}
