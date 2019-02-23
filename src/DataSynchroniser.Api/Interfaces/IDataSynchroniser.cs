using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Markets;
using Surveillance.Auditing.Context.Interfaces;

namespace DataSynchroniser.Api.Interfaces
{
    public interface IDataSynchroniser
    {
        Task Handle(
            string systemProcessOperationId,
            ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext,
            IReadOnlyCollection<MarketDataRequest> marketDataRequests);
    }
}
