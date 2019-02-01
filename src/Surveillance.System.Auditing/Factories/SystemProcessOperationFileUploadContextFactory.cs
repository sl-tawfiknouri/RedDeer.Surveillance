using System;
using Surveillance.Systems.Auditing.Context;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Systems.Auditing.Factories.Interfaces;
using Surveillance.Systems.Auditing.Logging.Interfaces;
using Surveillance.Systems.DataLayer.Repositories.Interfaces;

namespace Surveillance.Systems.Auditing.Factories
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
