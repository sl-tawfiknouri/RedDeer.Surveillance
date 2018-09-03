using System;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Trade.Interfaces;
using Surveillance.Recorders.Interfaces;
using Surveillance.Recorders.Projectors.Interfaces;

namespace Surveillance.Recorders
{
    public class RedDeerTradeRecorder : IRedDeerTradeRecorder
    {
        private readonly IRedDeerTradeFormatRepository _repository;
        private readonly IReddeerTradeFormatProjector _projector;
        private readonly ILogger _logger;

        public RedDeerTradeRecorder(
            IRedDeerTradeFormatRepository repository,
            IReddeerTradeFormatProjector projector,
            ILogger<RedDeerTradeRecorder> logger)
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
            _logger.LogError($"An exception occured in the reddeer trade recorder {error.ToString()}");
        }

        public async void OnNext(TradeOrderFrame value)
        {
            // project trade order frame into a ES document
            var projectedFrame = _projector.Project(value);

            if (projectedFrame == null)
            {
                return;
            }

            await _repository.Save(projectedFrame);
        }
    }
}
