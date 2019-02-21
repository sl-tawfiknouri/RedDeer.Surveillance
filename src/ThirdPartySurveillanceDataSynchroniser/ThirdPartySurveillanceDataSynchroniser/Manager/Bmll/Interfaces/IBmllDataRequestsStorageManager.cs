using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSynchroniser.Manager.Bmll.Interfaces
{
    public interface IBmllDataRequestsStorageManager
    {
        Task Store(IReadOnlyCollection<IGetTimeBarPair> timeBarPairs);
    }
}