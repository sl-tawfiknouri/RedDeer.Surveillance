using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationFileUploadContextFactory
    {
        ISystemProcessOperationUploadFileContext Build(ISystemProcessOperationContext operationContext);
    }
}