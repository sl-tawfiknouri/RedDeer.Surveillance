using System.Collections.Generic;
using System.Threading.Tasks;
using DataSynchroniser.Api.Interfaces;
using Domain.Markets;
using Surveillance.Auditing.Context.Interfaces;

namespace DataSynchroniser.Api.Bmll
{
    public class BmllDataSynchroniser : IDataSynchroniser
    {
        public async Task Handle(
            string systemProcessOperationId,
            ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext,
            IReadOnlyCollection<MarketDataRequest> marketDataRequests)
        {
        }
    }
}
