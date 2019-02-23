using System.Collections.Generic;
using Firefly.Service.Data.BMLL.Shared.Dtos;

namespace DataSynchroniser.Api.Bmll.Bmll.Interfaces
{
    public interface IBmllDataRequestsGetTimeBars
    {
        IReadOnlyCollection<IGetTimeBarPair> GetTimeBars(IReadOnlyCollection<MinuteBarRequestKeyDto> keys);
    }
}