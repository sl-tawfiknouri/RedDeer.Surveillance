using System;
using DomainV2.Equity.Frames;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.Recorders.Interfaces;

namespace Surveillance.Recorders
{
    public class RedDeerAuroraStockExchangeRecorder : IRedDeerAuroraStockExchangeRecorder
    {
        private readonly IReddeerMarketRepository _repository;
        private readonly ILogger<RedDeerAuroraStockExchangeRecorder> _logger;

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
                return;
            }

            try
            {
                _repository.Create(value).Wait();
            }
            catch (Exception e)
            {
                _logger.LogError($"RedDeer Aurora Stock Exchange Recorder had an error saving {e.Message}");
            }
        }
    }
}