namespace Surveillance.Auditing.Factories
{
    using System;

    using Surveillance.Auditing.Context;
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
    using Surveillance.Auditing.Factories.Interfaces;
    using Surveillance.Auditing.Logging.Interfaces;

    public class
        SystemProcessOperationDistributeRuleContextFactory : ISystemProcessOperationDistributeRuleContextFactory
    {
        private readonly IOperationLogging _operationLogging;

        private readonly ISystemProcessOperationDistributeRuleRepository _repository;

        public SystemProcessOperationDistributeRuleContextFactory(
            ISystemProcessOperationDistributeRuleRepository repository,
            IOperationLogging operationLogging)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public ISystemProcessOperationDistributeRuleContext Build(ISystemProcessOperationContext operationContext)
        {
            return new SystemProcessOperationDistributeRuleContext(
                operationContext,
                this._repository,
                this._operationLogging);
        }
    }
}