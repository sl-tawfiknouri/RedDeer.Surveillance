using Surveillance.System.Auditing.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationContext
    {
        ISystemProcessOperationFileUploadContext CreateFileUploadContext();
        ISystemProcessContext EndEvent();
        void StartEvent(ISystemProcessOperation processOperation);
    }
}