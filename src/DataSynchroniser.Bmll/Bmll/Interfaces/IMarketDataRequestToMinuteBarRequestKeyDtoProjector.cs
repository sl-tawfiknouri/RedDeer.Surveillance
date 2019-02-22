using System.Collections.Generic;
using Domain.Markets;
using Firefly.Service.Data.BMLL.Shared.Dtos;

namespace DataSynchroniser.Api.Bmll.Bmll.Interfaces
{
    public interface IMarketDataRequestToMinuteBarRequestKeyDtoProjector
    {
        IReadOnlyCollection<MinuteBarRequestKeyDto> ProjectToRequestKeys(IList<MarketDataRequest> bmllRequests);
    }
}