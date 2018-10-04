using System;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Context
{
    public class SystemProcessOperationRunRuleContext : ISystemProcessOperationRunRuleContext
    {
        private readonly ISystemProcessOperationRuleRunRepository _repository;
        private readonly ISystemProcessOperationContext _processOperationContext;
        private ISystemProcessOperationRuleRun _ruleRun;

        public SystemProcessOperationRunRuleContext(
            ISystemProcessOperationRuleRunRepository repository,
            ISystemProcessOperationContext processOperationContext)
        {
            _repository =
                repository
                ?? throw new ArgumentNullException(nameof(repository));

            _processOperationContext =
                processOperationContext
                ?? throw new ArgumentNullException(nameof(processOperationContext));
        }

        public void StartEvent(ISystemProcessOperationRuleRun ruleRun)
        {
            _ruleRun = ruleRun;
            _repository.Create(_ruleRun);
        }

        public ISystemProcessOperationContext EndEvent()
        {
            return _processOperationContext;
        }
    }
}
