using System;
using Surveillance.System.Auditing.Context;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Factories.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Factories
{
    public class SystemProcessOperationRunRuleContextFactory : ISystemProcessOperationRunRuleContextFactory
    {
        private readonly ISystemProcessOperationRuleRunRepository _repository;

        public SystemProcessOperationRunRuleContextFactory(ISystemProcessOperationRuleRunRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ISystemProcessOperationRunRuleContext Build(ISystemProcessOperationContext context)
        {
            return new SystemProcessOperationRunRuleContext(_repository, context);
        }
    }
}
