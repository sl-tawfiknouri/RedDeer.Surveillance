using System;
using Surveillance.Systems.Auditing.Context;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Systems.Auditing.Factories.Interfaces;
using Surveillance.Systems.Auditing.Logging.Interfaces;
using Surveillance.Systems.DataLayer.Repositories.Interfaces;

namespace Surveillance.Systems.Auditing.Factories
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
