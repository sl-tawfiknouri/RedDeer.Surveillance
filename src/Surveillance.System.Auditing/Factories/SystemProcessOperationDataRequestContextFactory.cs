using System;
using Surveillance.Systems.Auditing.Context;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Systems.Auditing.Factories.Interfaces;
using Surveillance.Systems.Auditing.Logging.Interfaces;
using Surveillance.Systems.DataLayer.Repositories.Interfaces;

namespace Surveillance.Systems.Auditing.Factories
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
