namespace Surveillance.Auditing.Context
{
    using System;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.DataLayer.Processes.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
    using Surveillance.Auditing.Logging.Interfaces;

    public class SystemProcessOperationRunRuleContext : ISystemProcessOperationRunRuleContext
    {
        private readonly IOperationLogging _operationLogging;

        private readonly ISystemProcessOperationContext _processOperationContext;

        private readonly ISystemProcessOperationRuleRunRepository _repository;

        private ISystemProcessOperationRuleRun _ruleRun;

        public SystemProcessOperationRunRuleContext(
            ISystemProcessOperationRuleRunRepository repository,
            ISystemProcessOperationContext processOperationContext,
            IOperationLogging operationLogging)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));

            this._processOperationContext = processOperationContext
                                            ?? throw new ArgumentNullException(nameof(processOperationContext));

            this._operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public string CorrelationId()
        {
            return this._ruleRun.CorrelationId ?? string.Empty;
        }

        public ISystemProcessOperationContext EndEvent()
        {
            return this._processOperationContext;
        }

        public void EventException(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            this._operationLogging.Log(new Exception(message), this._ruleRun);
        }

        public void EventException(Exception e)
        {
            this._operationLogging.Log(e, this._ruleRun);
        }

        public string Id()
        {
            return this._ruleRun?.Id.ToString() ?? string.Empty;
        }

        public bool IsBackTest()
        {
            return this._ruleRun.IsBackTest;
        }

        public string RuleParameterId()
        {
            return this._ruleRun.RuleParameterId ?? string.Empty;
        }

        public void StartEvent(ISystemProcessOperationRuleRun ruleRun)
        {
            this._ruleRun = ruleRun;
            this._repository.Create(this._ruleRun);
        }

        public ISystemProcessOperationContext SystemProcessOperationContext()
        {
            return this._processOperationContext;
        }
    }
}