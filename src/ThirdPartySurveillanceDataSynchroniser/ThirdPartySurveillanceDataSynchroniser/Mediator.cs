using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ThirdPartySurveillanceDataSynchroniser.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Services.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser
{
    public class Mediator : IMediator
    {
        private readonly IDataRequestsService _dataRequestsService;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IDataRequestsService dataRequestsService,
            ILogger<Mediator> logger)
        {
            _dataRequestsService = dataRequestsService ?? throw new ArgumentNullException(nameof(dataRequestsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation($"DATA SYNCHRONISER | process-id {Process.GetCurrentProcess().Id} | started-at {Process.GetCurrentProcess().StartTime}");
            _logger.LogInformation($"Mediator initiate beginning");

            _dataRequestsService.Initiate();

            _logger.LogInformation($"Mediator initiate complete");
        }
    }
}