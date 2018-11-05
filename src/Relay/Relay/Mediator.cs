using Microsoft.Extensions.Logging;
using Relay.Managers.Interfaces;
using System;
using Relay.Interfaces;
using Relay.S3_IO.Interfaces;

namespace Relay
{
    public class Mediator : IMediator
    {
        private readonly ITradeOrderStreamManager _tradeOrderStreamManager;
        private readonly IStockExchangeStreamManager _stockExchangeStreamManager;
        private readonly IS3FileUploadMonitoringProcess _s3FileUploadProcess;
        private readonly ILogger _logger;

        public Mediator(
            ITradeOrderStreamManager tradeOrderStreamManager,
            IStockExchangeStreamManager stockExchangeStreamManager,
            IS3FileUploadMonitoringProcess s3FileUploadProcess,
            ILogger<Mediator> logger)
        {
            _tradeOrderStreamManager =
                tradeOrderStreamManager
                ?? throw new ArgumentNullException(nameof(tradeOrderStreamManager));

            _stockExchangeStreamManager =
                stockExchangeStreamManager
                ?? throw new ArgumentNullException(nameof(stockExchangeStreamManager));

            _s3FileUploadProcess =
                s3FileUploadProcess
                ?? throw new ArgumentNullException(nameof(s3FileUploadProcess));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation("Initiating relay in mediator");

            _tradeOrderStreamManager.Initialise();
            _stockExchangeStreamManager.Initialise();
            _s3FileUploadProcess.Initialise();

            _logger.LogInformation("Completed initiating relay in mediator");
        }
    }
}