namespace Surveillance.Auditing.Factories
{
    using System;

    using Surveillance.Auditing.Context;
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
    using Surveillance.Auditing.Factories.Interfaces;
    using Surveillance.Auditing.Logging.Interfaces;

    public class SystemProcessOperationDataRequestContextFactory : ISystemProcessOperationDataRequestContextFactory
    {
        private readonly IOperationLogging _operationLogging;

        private readonly ISystemProcessOperationThirdPartyDataRequestRepository _repository;

        public SystemProcessOperationDataRequestContextFactory(
            ISystemProcessOperationThirdPartyDataRequestRepository repository,
            IOperationLogging operationLogging)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public ISystemProcessOperationThirdPartyDataRequestContext Build(
            ISystemProcessOperationContext operationContext)
        {
            return new SystemProcessOperationThirdPartyDataRequestContext(
                operationContext,
                this._repository,
                this._operationLogging);
        }
    }
}