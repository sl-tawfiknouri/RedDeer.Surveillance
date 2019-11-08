using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Surveillance.Data.Universe.Refinitiv.Interfaces
{
    public interface IRefinitivTickPriceHistoryApi
    {
        Task<IList<EndOfDaySecurityTimeBar>> GetInterdayTimeBars(DateTime startDay, DateTime endDay);
    }
}
