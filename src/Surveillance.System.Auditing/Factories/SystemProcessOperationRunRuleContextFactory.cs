using System;
using Surveillance.Auditing.Context;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
using Surveillance.Auditing.Factories.Interfaces;
using Surveillance.Auditing.Logging.Interfaces;

namespace Surveillance.Auditing.Factories
{
    public class SystemProcessOperationRunRuleContextFactory : ISystemProcessOperationRunRuleContextFactory
    {
        private readonly ISystemProcessOperationRuleRunRepository _repository;
        private readonly IOperationLogging _operationLogging;

        public SystemProcessOperationRunRuleContextFactory(
            ISystemProcessOperationRuleRunRepository repository,
            IOperationLogging operationLogging)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public ISystemProcessOperationRunRuleContext Build(ISystemProcessOperationContext context)
        {
            return new SystemProcessOperationRunRuleContext(_repository, context, _operationLogging);
        }
    }
}
