﻿namespace DataSynchroniser.Api.Markit
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DataSynchroniser.Api.Markit.Interfaces;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;

    public class MarkitDataSynchroniser : IMarkitDataSynchroniser
    {
        private readonly ILogger<MarkitDataSynchroniser> _logger;

        public MarkitDataSynchroniser(ILogger<MarkitDataSynchroniser> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(
            string systemProcessOperationId,
            ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext,
            IReadOnlyCollection<MarketDataRequest> marketDataRequests)
        {
            this._logger.LogInformation($"{nameof(MarkitDataSynchroniser)} Handle processing request");

            this._logger.LogInformation($"{nameof(MarkitDataSynchroniser)} Handle completed processing request");

            await Task.CompletedTask;
        }
    }
}