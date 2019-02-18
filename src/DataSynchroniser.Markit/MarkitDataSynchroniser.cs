using System.Collections.Generic;
using System.Threading.Tasks;
using DataSynchroniser.Api.Markit.Interfaces;
using Domain.Markets;
using Surveillance.Auditing.Context.Interfaces;

namespace DataSynchroniser.Api.Markit
{
    public class MarkitDataSynchroniser : IMarkitDataSynchroniser
    {
        public async Task Handle(
            string systemProcessOperationId,
            ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext,
            IReadOnlyCollection<MarketDataRequest> marketDataRequests)
        {
            
        }
    }
}
