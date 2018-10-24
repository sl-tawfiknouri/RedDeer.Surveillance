using System;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Context
{
    public class SystemProcessOperationDistributeRuleContext : ISystemProcessOperationDistributeRuleContext
    {
        private ISystemProcessOperationDistributeRule _distributeRule;
        private readonly ISystemProcessOperationContext _processOperationContext;
        private readonly ISystemProcessOperationDistributeRuleRepository _repository;

        public SystemProcessOperationDistributeRuleContext(
            ISystemProcessOperationContext processOperationContext,
            ISystemProcessOperationDistributeRuleRepository repository)
        {
            _processOperationContext =
                processOperationContext
                ?? throw new ArgumentNullException(nameof(processOperationContext));

            _repository =
                repository
                ?? throw new ArgumentNullException(nameof(repository));
        }

        public void StartEvent(ISystemProcessOperationDistributeRule distributeRule)
        {
            _distributeRule = distributeRule;
            _repository.Create(_distributeRule);
        }

        public ISystemProcessOperationContext EndEvent()
        {
            return _processOperationContext;
        }
    }
}
