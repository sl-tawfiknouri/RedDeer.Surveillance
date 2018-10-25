using System;
using Surveillance.System.Auditing.Context;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Factories.Interfaces;
using Surveillance.System.Auditing.Logging.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Factories
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
