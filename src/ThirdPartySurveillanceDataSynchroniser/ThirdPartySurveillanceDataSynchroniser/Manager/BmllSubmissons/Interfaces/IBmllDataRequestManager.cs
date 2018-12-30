using System.Collections.Generic;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.BmllSubmissons.Interfaces
{
    public interface IBmllDataRequestManager
    {
        void Submit(List<MarketDataRequestDataSource> bmllRequests);
    }
}
