namespace Surveillance.Auditing.Factories.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;

    public interface ISystemProcessOperationFileUploadContextFactory
    {
        ISystemProcessOperationUploadFileContext Build(ISystemProcessOperationContext operationContext);
    }
}