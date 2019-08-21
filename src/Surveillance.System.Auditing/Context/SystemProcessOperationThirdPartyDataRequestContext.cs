namespace Surveillance.Auditing.Context
{
    using System;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.DataLayer.Processes.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
    using Surveillance.Auditing.Logging.Interfaces;

    public class
        SystemProcessOperationThirdPartyDataRequestContext : ISystemProcessOperationThirdPartyDataRequestContext
    {
        private readonly IOperationLogging _operationLogging;

        private readonly ISystemProcessOperationContext _processOperationContext;

        private readonly ISystemProcessOperationThirdPartyDataRequestRepository _requestRepository;

        private ISystemProcessOperationThirdPartyDataRequest _request;

        public SystemProcessOperationThirdPartyDataRequestContext(
            ISystemProcessOperationContext processOperationContext,
            ISystemProcessOperationThirdPartyDataRequestRepository requestRepository,
            IOperationLogging operationLogging)
        {
            this._processOperationContext = processOperationContext
                                            ?? throw new ArgumentNullException(nameof(processOperationContext));
            this._requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
            this._operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public string Id => this._request?.Id.ToString() ?? string.Empty;

        public ISystemProcessOperationContext EndEvent()
        {
            return this._processOperationContext;
        }

        public void EventError(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            this._operationLogging.Log(new Exception(message), this._request);
        }

        public void StartEvent(ISystemProcessOperationThirdPartyDataRequest request)
        {
            this._request = request;
            this._requestRepository?.Create(request);
        }
    }
}