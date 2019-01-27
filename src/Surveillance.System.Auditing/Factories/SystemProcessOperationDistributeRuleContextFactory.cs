﻿using System;
using Surveillance.Systems.Auditing.Context;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Systems.Auditing.Factories.Interfaces;
using Surveillance.Systems.Auditing.Logging.Interfaces;
using Surveillance.Systems.DataLayer.Repositories.Interfaces;

namespace Surveillance.Systems.Auditing.Factories
{
    public class SystemProcessOperationDistributeRuleContextFactory : ISystemProcessOperationDistributeRuleContextFactory
    {
        private readonly ISystemProcessOperationDistributeRuleRepository _repository;
        private readonly IOperationLogging _operationLogging;

        public SystemProcessOperationDistributeRuleContextFactory(
            ISystemProcessOperationDistributeRuleRepository repository,
            IOperationLogging operationLogging)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public ISystemProcessOperationDistributeRuleContext Build(ISystemProcessOperationContext operationContext)
        {
            return new SystemProcessOperationDistributeRuleContext(operationContext, _repository, _operationLogging);
        }
    }
}
