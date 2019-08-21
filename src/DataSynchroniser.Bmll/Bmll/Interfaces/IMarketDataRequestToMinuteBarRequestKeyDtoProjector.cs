namespace DataSynchroniser.Api.Bmll.Bmll.Interfaces
{
    using System.Collections.Generic;

    using Firefly.Service.Data.BMLL.Shared.Dtos;

    using SharedKernel.Contracts.Markets;

    public interface IMarketDataRequestToMinuteBarRequestKeyDtoProjector
    {
        IReadOnlyCollection<MinuteBarRequestKeyDto> ProjectToRequestKeys(IList<MarketDataRequest> bmllRequests);
    }
}