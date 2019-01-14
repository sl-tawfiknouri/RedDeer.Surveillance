﻿using System.Threading.Tasks;
using Surveillance.System.Auditing.Context.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Interfaces
{
    public interface IDataRequestManager
    {
        Task Handle(string ruleRunId, ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext);
    }
}