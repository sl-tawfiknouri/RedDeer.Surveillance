using Surveillance.System.Auditing.Context.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Interfaces
{
    public interface IDataRequestManager
    {
        void Handle(string ruleRunId, ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext);
    }
}