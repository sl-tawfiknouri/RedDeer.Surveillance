using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.BmllSubmissons.Interfaces
{
    public interface IBmllDataRequestManager
    {
        Task Submit(List<MarketDataRequestDataSource> bmllRequests);
    }
}
