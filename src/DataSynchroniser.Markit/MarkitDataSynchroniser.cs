using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSynchroniser.Api.Markit.Interfaces;
using Domain.Markets;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;

namespace DataSynchroniser.Api.Markit
{
    public class MarkitDataSynchroniser : IMarkitDataSynchroniser
    {
        private readonly ILogger<MarkitDataSynchroniser> _logger;

        public MarkitDataSynchroniser(ILogger<MarkitDataSynchroniser> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(
            string systemProcessOperationId,
            ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext,
            IReadOnlyCollection<MarketDataRequest> marketDataRequests)
        {
            _logger.LogInformation($"MarkitDataSynchroniser Handle processing request");



            _logger.LogInformation($"MarkitDataSynchroniser Handle completed processing request");
        }
    }
}
