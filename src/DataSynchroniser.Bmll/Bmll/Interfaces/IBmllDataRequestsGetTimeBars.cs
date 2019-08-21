namespace DataSynchroniser.Api.Bmll.Bmll.Interfaces
{
    using System.Collections.Generic;

    using Firefly.Service.Data.BMLL.Shared.Dtos;

    public interface IBmllDataRequestsGetTimeBars
    {
        IReadOnlyCollection<IGetTimeBarPair> GetTimeBars(IReadOnlyCollection<MinuteBarRequestKeyDto> keys);
    }
}