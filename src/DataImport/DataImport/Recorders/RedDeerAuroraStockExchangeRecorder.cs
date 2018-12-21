using System;
using DataImport.Recorders.Interfaces;
using DomainV2.Equity.Frames;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Market.Interfaces;

namespace DataImport.Recorders
{
    public class RedDeerAuroraStockExchangeRecorder : IRedDeerAuroraStockExchangeRecorder
    {
        private readonly IReddeerMarketRepository _repository;
        private readonly ILogger<RedDeerAuroraStockExchangeRecorder> _logger;
        private readonly object _lock = new object();

        public RedDeerAuroraStockExchangeRecorder(
            IReddeerMarketRepository repository,
            ILogger<RedDeerAuroraStockExchangeRecorder> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            _logger.LogError($"RedDeerAuroraStockExchangeRecorder {error.Message}");
        }

        public void OnNext(ExchangeFrame value)
        {
            if (value == null)
            {
                _logger.LogError($"RedDeerAuroraStockExchangeRecorder OnNext was passed a null value. Returning.");
                return;
            }

            try
            {
                lock (_lock)
                {
                    _logger.LogInformation($"RedDeerAuroraStockExchangeRecorder {value.TimeStamp} {value.Exchange?.MarketIdentifierCode} Passing market data to repository");
                    _repository.Create(value).Wait();
                    _logger.LogInformation($"RedDeerAuroraStockExchangeRecorder {value.TimeStamp} {value.Exchange?.MarketIdentifierCode} Completed passing market data to repository");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"RedDeer Aurora Stock Exchange Recorder had an error saving {e.Message}");
            }
        }
    }
}