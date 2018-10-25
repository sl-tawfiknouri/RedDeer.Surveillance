using System;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Logging.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Context
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

        public ISystemProcessOperationRunRuleContext UpdateAlertEvent(int alerts)
        {
            _ruleRun.Alerts = alerts;
            _repository.Update(_ruleRun);
            return this;
        }

        public void EventException(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _operationLogging.Log(new Exception(message));
        }

        public void EventException(Exception e)
        {
            _operationLogging.Log(e);
        }

        public ISystemProcessOperationContext EndEvent()
        {
            return _processOperationContext;
        }
    }
}
