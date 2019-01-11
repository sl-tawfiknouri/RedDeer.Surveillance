﻿using System;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Logging.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Context
{
    public class SystemProcessOperationThirdPartyDataRequestContext : ISystemProcessOperationThirdPartyDataRequestContext
    {
        private readonly ISystemProcessOperationContext _processOperationContext;
        private readonly ISystemProcessOperationThirdPartyDataRequestRepository _requestRepository;
        private readonly IOperationLogging _operationLogging;

        private ISystemProcessOperationThirdPartyDataRequest _request;

        public SystemProcessOperationThirdPartyDataRequestContext(
            ISystemProcessOperationContext processOperationContext,
            ISystemProcessOperationThirdPartyDataRequestRepository requestRepository,
            IOperationLogging operationLogging)
        {
            _processOperationContext = processOperationContext ?? throw new ArgumentNullException(nameof(processOperationContext));
            _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
            _operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public void StartEvent(ISystemProcessOperationThirdPartyDataRequest request)
        {
            _request = request;
            _requestRepository?.Create(request);
        }

        public void EventError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _operationLogging.Log(new Exception(message), _request);
        }

        public string Id => _request?.Id.ToString() ?? string.Empty;

        public ISystemProcessOperationContext EndEvent()
        {
            return _processOperationContext;
        }
    }
}
