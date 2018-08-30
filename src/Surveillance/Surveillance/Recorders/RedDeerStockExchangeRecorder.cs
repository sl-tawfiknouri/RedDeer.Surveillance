using Domain.Equity.Trading.Frames;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.ElasticSearch.Market.Interfaces;
using Surveillance.Recorders.Interfaces;
using Surveillance.Recorders.Projectors.Interfaces;
using System;

namespace Surveillance.Recorders
{
    public class RedDeerStockExchangeRecorder : IRedDeerStockExchangeRecorder
    {
        private IRedDeerMarketExchangeFormatRepository _repository;
        private IReddeerMarketExchangeFormatProjector _projector;
        private ILogger _logger;

        public RedDeerStockExchangeRecorder(
            IRedDeerMarketExchangeFormatRepository repository,
            IReddeerMarketExchangeFormatProjector projector,
            ILogger<RedDeerStockExchangeRecorder> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _projector = projector ?? throw new ArgumentNullException(nameof(projector));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            _logger.LogError($"An exception occured in the reddeer stock exchange recorder {error.ToString()}");
        }

        public async void OnNext(ExchangeFrame value)
        {
            var projectedValue = _projector.Project(value);

            if (projectedValue == null)
            {
                return;
            }

            await _repository.Save(projectedValue);
        }
    }
}
