using System;
using System.Diagnostics;
using DataSynchroniser.Interfaces;
using DataSynchroniser.Queues.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Utilities.Interfaces;

namespace DataSynchroniser
{
    public class Mediator : IMediator
    {
        private readonly IDataRequestSubscriber _dataRequestSubscriber;
        private readonly IApplicationHeartbeatService _heartbeatService;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IDataRequestSubscriber dataRequestSubscriber,
            IApplicationHeartbeatService heartbeatService,
            ILogger<Mediator> logger)
        {
            _dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            _heartbeatService = heartbeatService ?? throw new ArgumentNullException(nameof(heartbeatService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation($"DATA SYNCHRONISER | process-id {Process.GetCurrentProcess().Id} | started-at {Process.GetCurrentProcess().StartTime}");

            _heartbeatService.Initialise();
            _dataRequestSubscriber.Initiate();

            _logger.LogInformation($"Mediator initiate complete");
        }
    }
}