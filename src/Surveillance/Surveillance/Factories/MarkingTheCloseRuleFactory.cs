﻿using System;
using Microsoft.Extensions.Logging;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.MarkingTheClose;
using Surveillance.Rules.MarkingTheClose.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories
{
    public class MarkingTheCloseRuleFactory : IMarkingTheCloseRuleFactory
    {
        private readonly IMarkingTheCloseMessageSender _messageSender;
        private readonly ILogger<MarkingTheCloseRule> _logger;

        public MarkingTheCloseRuleFactory(
            ILogger<MarkingTheCloseRule> logger,
            IMarkingTheCloseMessageSender messageSender)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public IMarkingTheCloseRule Build(IMarkingTheCloseParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx)
        {
            return new MarkingTheCloseRule(parameters, _messageSender, ruleCtx, _logger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
