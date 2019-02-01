using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Systems.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationFileUploadContextFactory
    {
        ISystemProcessOperationUploadFileContext Build(ISystemProcessOperationContext operationContext);
    }
}