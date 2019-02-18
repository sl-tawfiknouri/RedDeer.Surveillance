using System;
using Surveillance.Auditing.Context;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
using Surveillance.Auditing.Factories.Interfaces;
using Surveillance.Auditing.Logging.Interfaces;

namespace Surveillance.Auditing.Factories
{
    public class SystemProcessOperationFileUploadContextFactory : ISystemProcessOperationFileUploadContextFactory
    {
        private readonly ISystemProcessOperationUploadFileRepository _repository;
        private readonly IOperationLogging _operationLogging;

        public SystemProcessOperationFileUploadContextFactory(
            IOperationLogging operationLogging,
            ISystemProcessOperationUploadFileRepository repository)
        {
            _operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ISystemProcessOperationUploadFileContext Build(ISystemProcessOperationContext operationContext)
        {
            return new SystemProcessOperationUploadFileContext(operationContext, _operationLogging, _repository);
        }
    }
}
