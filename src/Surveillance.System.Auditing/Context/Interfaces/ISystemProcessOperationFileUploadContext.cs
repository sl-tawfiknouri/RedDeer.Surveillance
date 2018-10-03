using Surveillance.System.Auditing.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationFileUploadContext
    {
        ISystemProcessOperationContext EndEvent();
        void StartEvent(ISystemProcessOperationFileUpload fileUpload);
    }
}