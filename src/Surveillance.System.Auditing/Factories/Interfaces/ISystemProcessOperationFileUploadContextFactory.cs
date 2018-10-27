using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.System.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationFileUploadContextFactory
    {
        ISystemProcessOperationUploadFileContext Build(ISystemProcessOperationContext operationContext);
    }
}