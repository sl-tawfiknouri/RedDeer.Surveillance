namespace DataSynchroniser
{
    using System;
    using System.Diagnostics;

    using DataSynchroniser.Interfaces;
    using DataSynchroniser.Queues.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Utilities.Interfaces;

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
            this._dataRequestSubscriber =
                dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            this._heartbeatService = heartbeatService ?? throw new ArgumentNullException(nameof(heartbeatService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            this._logger.LogInformation(
                $"DATA SYNCHRONISER | process-id {Process.GetCurrentProcess().Id} | started-at {Process.GetCurrentProcess().StartTime}");

            this._heartbeatService.Initialise();
            this._dataRequestSubscriber.Initiate();

            this._logger.LogInformation($"{nameof(Mediator)} initiate complete");
        }
    }
}