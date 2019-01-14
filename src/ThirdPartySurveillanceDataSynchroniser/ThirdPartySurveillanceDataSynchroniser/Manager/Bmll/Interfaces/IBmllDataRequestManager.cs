﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces
{
    public interface IBmllDataRequestManager
    {
        Task Submit(string ruleRunId, List<MarketDataRequestDataSource> bmllRequests);
    }
}