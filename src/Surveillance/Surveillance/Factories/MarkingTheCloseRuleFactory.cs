using System;
using Microsoft.Extensions.Logging;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules.Marking_The_Close;
using Surveillance.Rules.Marking_The_Close.Interfaces;

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

        public IMarkingTheCloseRule Build()
        {
            var parameters = new MarkingTheCloseParameters();
            var rule = new MarkingTheCloseRule(parameters, _messageSender, _logger);

            return rule;
        }
    }
}
