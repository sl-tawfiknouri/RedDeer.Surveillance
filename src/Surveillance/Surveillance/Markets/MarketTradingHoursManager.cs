using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Markets
{
    public class MarketTradingHoursManager : IMarketTradingHoursManager
    {
        private readonly IMarketOpenCloseApiCachingDecoratorRepository _repository;
        private readonly ILogger<MarketTradingHoursManager> _logger;

        public MarketTradingHoursManager(
            IMarketOpenCloseApiCachingDecoratorRepository repository,
            ILogger<MarketTradingHoursManager> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ITradingHours Get(string marketIdentifierCode)
        {
            if (string.IsNullOrWhiteSpace(marketIdentifierCode))
            {
                _logger.LogInformation($"MarketTradingHoursManager received a null or empty MIC {marketIdentifierCode}");
                return new TradingHours { Mic = marketIdentifierCode, IsValid = false };
            }

            var resultTask = _repository.Get();
            var result = resultTask.Result;

            var exchange = result.FirstOrDefault(res => string.Equals(res.Code, marketIdentifierCode, StringComparison.InvariantCultureIgnoreCase));

            if (exchange == null)
            {
                _logger.LogError($"MarketTradingHoursManager could not find a match for {marketIdentifierCode}");

                return new TradingHours { Mic = marketIdentifierCode, IsValid = false };
            }

            _logger.LogInformation($"MarketTradingHoursManager found a match for {marketIdentifierCode}");

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
