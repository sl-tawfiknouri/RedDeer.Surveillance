namespace Surveillance.Auditing.Factories
{
    using System;

    using Surveillance.Auditing.Context;
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
    using Surveillance.Auditing.Factories.Interfaces;
    using Surveillance.Auditing.Logging.Interfaces;

    public class SystemProcessOperationFileUploadContextFactory : ISystemProcessOperationFileUploadContextFactory
    {
        private readonly IOperationLogging _operationLogging;

        private readonly ISystemProcessOperationUploadFileRepository _repository;

        public SystemProcessOperationFileUploadContextFactory(
            IOperationLogging operationLogging,
            ISystemProcessOperationUploadFileRepository repository)
        {
            this._operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ISystemProcessOperationUploadFileContext Build(ISystemProcessOperationContext operationContext)
        {
            return new SystemProcessOperationUploadFileContext(
                operationContext,
                this._operationLogging,
                this._repository);
        }
    }
}