using System;
using Surveillance.System.Auditing.Context;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Factories.Interfaces;
using Surveillance.System.Auditing.Logging.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Factories
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
