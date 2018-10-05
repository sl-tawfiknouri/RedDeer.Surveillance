﻿using System;
using Surveillance.System.Auditing.Context;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Factories.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Factories
{
    public class SystemProcessOperationContextFactory : ISystemProcessOperationContextFactory
    {
        private readonly ISystemProcessOperationRepository _operationRepository;
        private readonly ISystemProcessOperationRunRuleContextFactory _ruleRunFactory;
        private readonly ISystemProcessOperationDistributeRuleContextFactory _distributeRuleFactory;

        public SystemProcessOperationContextFactory(
            ISystemProcessOperationRepository operationContext,
            ISystemProcessOperationRunRuleContextFactory ruleRunRepository,
            ISystemProcessOperationDistributeRuleContextFactory distributeRuleFactory)
        {
            _operationRepository = operationContext ?? throw new ArgumentNullException(nameof(operationContext));
            _ruleRunFactory = ruleRunRepository ?? throw new ArgumentNullException(nameof(ruleRunRepository));
            _distributeRuleFactory = distributeRuleFactory ?? throw new ArgumentNullException(nameof(distributeRuleFactory));
        }

        public ISystemProcessOperationContext Build(ISystemProcessContext context)
        {
            return new SystemProcessOperationContext(
                context,
                _operationRepository,
                _ruleRunFactory,
                _distributeRuleFactory);
        }
    }
}