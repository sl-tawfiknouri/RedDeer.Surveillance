using System;
using Surveillance.Auditing.Context;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
using Surveillance.Auditing.Factories.Interfaces;
using Surveillance.Auditing.Logging.Interfaces;

namespace Surveillance.Auditing.Factories
{
    public class SystemProcessOperationDistributeRuleContextFactory : ISystemProcessOperationDistributeRuleContextFactory
    {
        private readonly ISystemProcessOperationDistributeRuleRepository _repository;
        private readonly IOperationLogging _operationLogging;

        public SystemProcessOperationDistributeRuleContextFactory(
            ISystemProcessOperationDistributeRuleRepository repository,
            IOperationLogging operationLogging)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public ISystemProcessOperationDistributeRuleContext Build(ISystemProcessOperationContext operationContext)
        {
            return new SystemProcessOperationDistributeRuleContext(operationContext, _repository, _operationLogging);
        }
    }
}
