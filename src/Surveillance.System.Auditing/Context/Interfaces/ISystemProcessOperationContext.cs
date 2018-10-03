using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationContext
    {
        ISystemProcessOperationFileUploadContext CreateFileUploadContext();
        ISystemProcessOperationDistributeRuleContext CreateDistributeRuleContext();
        ISystemProcessContext EndEvent();
        void StartEvent(ISystemProcessOperation processOperation);
    }
}