using System;
using Surveillance.System.Auditing.Context;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Factories.Interfaces;
using Surveillance.System.Auditing.Logging.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Factories
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
