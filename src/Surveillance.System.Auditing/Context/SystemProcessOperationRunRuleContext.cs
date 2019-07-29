using System;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
using Surveillance.Auditing.Logging.Interfaces;

namespace Surveillance.Auditing.Context
{
    public class SystemProcessOperationRunRuleContext : ISystemProcessOperationRunRuleContext
    {
        private readonly ISystemProcessOperationRuleRunRepository _repository;
        private readonly ISystemProcessOperationContext _processOperationContext;
        private ISystemProcessOperationRuleRun _ruleRun;
        private readonly IOperationLogging _operationLogging;

        public SystemProcessOperationRunRuleContext(
            ISystemProcessOperationRuleRunRepository repository,
            ISystemProcessOperationContext processOperationContext,
            IOperationLogging operationLogging)
        {
            _repository =
                repository
                ?? throw new ArgumentNullException(nameof(repository));

            _processOperationContext =
                processOperationContext
                ?? throw new ArgumentNullException(nameof(processOperationContext));

            _operationLogging =
                operationLogging
                ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public void StartEvent(ISystemProcessOperationRuleRun ruleRun)
        {
            _ruleRun = ruleRun;
            _repository.Create(_ruleRun);
        }

        public void EventException(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _operationLogging.Log(new Exception(message), _ruleRun);
        }

        public void EventException(Exception e)
        {
            _operationLogging.Log(e, _ruleRun);
        }

        public ISystemProcessOperationContext EndEvent()
        {
            return _processOperationContext;
        }

        public string Id()
        {
            return _ruleRun?.Id.ToString() ?? string.Empty;
        }

        public string CorrelationId()
        {
            return _ruleRun.CorrelationId ?? string.Empty;
        }

        public string RuleParameterId()
        {
            return _ruleRun.RuleParameterId ?? string.Empty;
        }

        public bool IsBackTest()
        {
            return _ruleRun.IsBackTest;
        }

        public ISystemProcessOperationContext SystemProcessOperationContext()
        {
            return _processOperationContext;
        }
    }
}
