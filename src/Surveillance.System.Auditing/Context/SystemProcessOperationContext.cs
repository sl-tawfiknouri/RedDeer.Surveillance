﻿using System;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Factories.Interfaces;
using Surveillance.System.DataLayer.Processes;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Context
{
    public class SystemProcessOperationContext : ISystemProcessOperationContext
    {
        private ISystemProcessOperation _systemProcessOperation;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly ISystemProcessOperationRepository _systemProcessOperationRepository;
        private readonly ISystemProcessOperationRunRuleContextFactory _runRuleContextFactory;
        private readonly ISystemProcessOperationDistributeRuleContextFactory _distributeRuleContextFactory;

        public SystemProcessOperationContext(
            ISystemProcessContext systemProcessContext,
            ISystemProcessOperationRepository systemProcessOperationRepository,
            ISystemProcessOperationRunRuleContextFactory runRuleContextFactory,
            ISystemProcessOperationDistributeRuleContextFactory distributeRuleContextFactory)
        {
            _systemProcessContext =
                systemProcessContext
                ?? throw new ArgumentNullException(nameof(systemProcessContext));

            _systemProcessOperationRepository =
                systemProcessOperationRepository
                ?? throw new ArgumentNullException(nameof(systemProcessOperationRepository));

            _runRuleContextFactory =
                runRuleContextFactory
                ?? throw new ArgumentNullException(nameof(runRuleContextFactory));

            _distributeRuleContextFactory =
                distributeRuleContextFactory
                ?? throw new ArgumentNullException(nameof(distributeRuleContextFactory));
        }

        public ISystemProcessOperationDistributeRuleContext CreateDistributeRuleContext()
        {
            return _distributeRuleContextFactory.Build(this);
        }

        public ISystemProcessOperationDistributeRuleContext CreateAndStartDistributeRuleContext(
            DateTime? initialStart,
            DateTime? initialEnd,
            string rules)
        {
            var op = new SystemProcessOperationDistributeRule
            {
                SystemProcessOperationId = _systemProcessOperation.Id,
                InitialStart = initialStart,
                InitialEnd = initialEnd,
                RulesDistributed = rules
            };

            var ctx = _distributeRuleContextFactory.Build(this);
            ctx.StartEvent(op);

            return ctx;
        }

        public ISystemProcessOperationRunRuleContext CreateRuleRunContext()
        {
            return _runRuleContextFactory.Build(this);
        }

        public ISystemProcessOperationRunRuleContext CreateAndStartRuleRunContext(
            string ruleDescription,
            string ruleVersion,
            DateTime ruleScheduleBegin,
            DateTime ruleScheduleEnd)
        {
            var ctx = _runRuleContextFactory.Build(this);
            var startEvent = new SystemProcessOperationRuleRun
            {
                SystemProcessOperationId = _systemProcessOperation.Id,
                RuleDescription = ruleDescription,
                RuleVersion = ruleVersion,
                ScheduleRuleStart = ruleScheduleBegin,
                ScheduleRuleEnd = ruleScheduleEnd
            };

            ctx.StartEvent(startEvent);

            return ctx;
        }

        public void StartEvent(ISystemProcessOperation processOperation)
        {
            _systemProcessOperation = processOperation;
            _systemProcessOperationRepository.Create(processOperation);
        }

        public ISystemProcessContext EndEvent()
        {
            _systemProcessOperation.OperationEnd = DateTime.UtcNow;
            _systemProcessOperationRepository.Update(_systemProcessOperation);
            return _systemProcessContext;
        }
    }
}
