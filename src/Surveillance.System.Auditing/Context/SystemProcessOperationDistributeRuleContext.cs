using System;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Logging.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Context
{
    public class SystemProcessOperationDistributeRuleContext : ISystemProcessOperationDistributeRuleContext
    {
        private ISystemProcessOperationDistributeRule _distributeRule;
        private readonly ISystemProcessOperationContext _processOperationContext;
        private readonly ISystemProcessOperationDistributeRuleRepository _repository;
        private readonly IOperationLogging _operationLogging;

        public SystemProcessOperationDistributeRuleContext(
            ISystemProcessOperationContext processOperationContext,
            ISystemProcessOperationDistributeRuleRepository repository,
            IOperationLogging operationLogging)
        {
            _processOperationContext =
                processOperationContext
                ?? throw new ArgumentNullException(nameof(processOperationContext));

            _repository =
                repository
                ?? throw new ArgumentNullException(nameof(repository));

            _operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public void StartEvent(ISystemProcessOperationDistributeRule distributeRule)
        {
            _distributeRule = distributeRule;
            _repository.Create(_distributeRule);
        }

        public void EventError(string message)
        {
            _operationLogging.Log(new Exception(message));
        }

        public ISystemProcessOperationContext EndEvent()
        {
            return _processOperationContext;
        }
    }
}
