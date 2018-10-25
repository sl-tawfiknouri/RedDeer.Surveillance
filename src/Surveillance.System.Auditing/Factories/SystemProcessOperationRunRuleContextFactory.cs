using System;
using Surveillance.System.Auditing.Context;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Factories.Interfaces;
using Surveillance.System.Auditing.Logging.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Factories
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
