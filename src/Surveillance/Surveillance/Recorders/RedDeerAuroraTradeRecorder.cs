using System;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
using Surveillance.Recorders.Interfaces;

namespace Surveillance.Recorders
{
    public class RedDeerAuroraTradeRecorder : IRedDeerAuroraTradeRecorder
    {
        private readonly IReddeerTradeRepository _repository;
        private readonly ILogger _logger;

        public RedDeerAuroraTradeRecorder(
            IReddeerTradeRepository repository,
            ILogger<RedDeerAuroraTradeRecorder> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            _logger.LogError($"An exception occured in the reddeer trade recorder {error}");
        }

        public async void OnNext(Order value)
        {
            if (value == null)
            {
                return;
            }

            await _repository.Create(value);
        }
    }
}
