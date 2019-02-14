using System;
using Surveillance.Auditing.Context;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
using Surveillance.Auditing.Factories.Interfaces;
using Surveillance.Auditing.Logging.Interfaces;

namespace Surveillance.Auditing.Factories
{
    public class SystemProcessOperationDataRequestContextFactory : ISystemProcessOperationDataRequestContextFactory
    {
        private readonly ISystemProcessOperationThirdPartyDataRequestRepository _repository;
        private readonly IOperationLogging _operationLogging;

        public SystemProcessOperationDataRequestContextFactory(
            ISystemProcessOperationThirdPartyDataRequestRepository repository,
            IOperationLogging operationLogging)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public ISystemProcessOperationThirdPartyDataRequestContext Build(
            ISystemProcessOperationContext operationContext)
        {
            return new SystemProcessOperationThirdPartyDataRequestContext(operationContext, _repository, _operationLogging);
        }
    }
}
