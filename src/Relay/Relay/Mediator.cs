using Microsoft.Extensions.Logging;
using Relay.Managers.Interfaces;
using System;

namespace Relay
{
    public class Mediator : IMediator
    {
        private ITradeOrderStreamManager _tradeOrderStreamManager;
        private IStockExchangeStreamManager _stockExchangeStreamManager;
        private ILogger _logger;

        public Mediator(
            ITradeOrderStreamManager tradeOrderStreamManager,
            IStockExchangeStreamManager stockExchangeStreamManager,
            ILogger<Mediator> logger)
        {
            _tradeOrderStreamManager =
                tradeOrderStreamManager
                ?? throw new ArgumentNullException(nameof(tradeOrderStreamManager));

            _stockExchangeStreamManager =
                stockExchangeStreamManager
                ?? throw new ArgumentNullException(nameof(stockExchangeStreamManager));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation("Initiating relay in mediator");

            _tradeOrderStreamManager.Initialise();
            _stockExchangeStreamManager.Initialise();

            _logger.LogInformation("Completed initiating relay in mediator");
        }
    }
}