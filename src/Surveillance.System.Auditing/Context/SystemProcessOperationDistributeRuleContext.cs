namespace Surveillance.Auditing.Context
{
    using System;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.DataLayer.Processes.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
    using Surveillance.Auditing.Logging.Interfaces;

    public class SystemProcessOperationDistributeRuleContext : ISystemProcessOperationDistributeRuleContext
    {
        private readonly IOperationLogging _operationLogging;

        private readonly ISystemProcessOperationContext _processOperationContext;

        private readonly ISystemProcessOperationDistributeRuleRepository _repository;

        private ISystemProcessOperationDistributeRule _distributeRule;

        public SystemProcessOperationDistributeRuleContext(
            ISystemProcessOperationContext processOperationContext,
            ISystemProcessOperationDistributeRuleRepository repository,
            IOperationLogging operationLogging)
        {
            this._processOperationContext = processOperationContext
                                            ?? throw new ArgumentNullException(nameof(processOperationContext));

            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));

            this._operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public string Id => this._distributeRule?.Id.ToString() ?? string.Empty;

        public ISystemProcessOperationContext EndEvent()
        {
            return this._processOperationContext;
        }

        public void EventError(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            this._operationLogging.Log(new Exception(message), this._distributeRule);
        }

        public void StartEvent(ISystemProcessOperationDistributeRule distributeRule)
        {
            this._distributeRule = distributeRule;
            this._repository.Create(this._distributeRule);
        }
    }
}