using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSynchroniser.Api.Bmll.Bmll.Interfaces
{
    public interface IBmllDataRequestsStorageManager
    {
        Task Store(IReadOnlyCollection<IGetTimeBarPair> timeBarPairs);
    }
}