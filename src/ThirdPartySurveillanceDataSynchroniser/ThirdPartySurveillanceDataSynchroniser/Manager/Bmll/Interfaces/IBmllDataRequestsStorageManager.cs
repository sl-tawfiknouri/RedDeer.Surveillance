using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces
{
    public interface IBmllDataRequestsStorageManager
    {
        Task Store(IReadOnlyCollection<IGetTimeBarPair> timeBarPairs);
    }
}