namespace Surveillance.Auditing.Factories
{
    using System;

    using Surveillance.Auditing.Context;
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
    using Surveillance.Auditing.Factories.Interfaces;
    using Surveillance.Auditing.Logging.Interfaces;

    public class SystemProcessOperationRunRuleContextFactory : ISystemProcessOperationRunRuleContextFactory
    {
        private readonly IOperationLogging _operationLogging;

        private readonly ISystemProcessOperationRuleRunRepository _repository;

        public SystemProcessOperationRunRuleContextFactory(
            ISystemProcessOperationRuleRunRepository repository,
            IOperationLogging operationLogging)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public ISystemProcessOperationRunRuleContext Build(ISystemProcessOperationContext context)
        {
            return new SystemProcessOperationRunRuleContext(this._repository, context, this._operationLogging);
        }
    }
}