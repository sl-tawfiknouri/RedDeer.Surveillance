using System;
using Surveillance.System.Auditing.Context;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Factories.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Factories
{
    public class SystemProcessOperationDistributeRuleContextFactory : ISystemProcessOperationDistributeRuleContextFactory
    {
        private readonly ISystemProcessOperationDistributeRuleRepository _repository;

        public SystemProcessOperationDistributeRuleContextFactory(
            ISystemProcessOperationDistributeRuleRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ISystemProcessOperationDistributeRuleContext Build(ISystemProcessOperationContext operationContext)
        {
            return new SystemProcessOperationDistributeRuleContext(operationContext, _repository);
        }
    }
}
