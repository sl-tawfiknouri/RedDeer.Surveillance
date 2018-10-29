﻿using System;
using Microsoft.Extensions.Logging;
using Surveillance.Mappers.Interfaces;
using Surveillance.MessageBus_IO.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits
{
    public class HighProfitMessageSender : BaseMessageSender, IHighProfitMessageSender
    {
        public HighProfitMessageSender(
            ITradeOrderDataItemDtoMapper dtoMapper,
            ILogger<HighProfitMessageSender> logger,
            ICaseMessageSender caseMessageSender)
            : base(dtoMapper, "Automated High Profit Rule Breach Detected", "High Profit Message Sender", logger, caseMessageSender)
        { }

        public void Send(IHighProfitRuleBreach ruleBreach, ISystemProcessOperationRunRuleContext opCtx)
        {
            var description = BuildDescription(ruleBreach);
            Send(ruleBreach, description, opCtx);
        }

        private string BuildDescription(IHighProfitRuleBreach ruleBreach)
        {
            var highRelativeProfitAsPercentage =
                Math.Round(
                    (ruleBreach.RelativeProfits.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var highRelativeProfitAsPercentageSetByUser =
                Math.Round(
                    (ruleBreach.Parameters.HighProfitPercentageThreshold.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var highRelativeProfitSection =
                HighRelativeProfitText(ruleBreach, highRelativeProfitAsPercentage, highRelativeProfitAsPercentageSetByUser);
            var highAbsoluteProfitSection = HighAbsoluteProfitText(ruleBreach);

            return $"High profit rule breach detected for {ruleBreach.Security.Name} ({ruleBreach.Security.Identifiers}).{highRelativeProfitSection}{highAbsoluteProfitSection}";
        }

        private string HighRelativeProfitText(
            IHighProfitRuleBreach ruleBreach,
            decimal highRelativeProfitAsPercentage,
            decimal highRelativeProfitAsPercentageSetByUser)
        {
            return ruleBreach.HasRelativeProfitBreach
                    ? $" There was a high profit ratio of {highRelativeProfitAsPercentage}% which exceeded the configured high profit ratio percentage threshold of {highRelativeProfitAsPercentageSetByUser}%."
                    : string.Empty;
        }

        private string HighAbsoluteProfitText(IHighProfitRuleBreach ruleBreach)
        {
            return ruleBreach.HasAbsoluteProfitBreach
                ? $" There was a high profit of {ruleBreach.AbsoluteProfits} ({ruleBreach.AbsoluteProfitCurrency}) which exceeded the configured profit limit of {ruleBreach.Parameters.HighProfitAbsoluteThreshold.GetValueOrDefault(0)}({ruleBreach.Parameters.HighProfitAbsoluteThresholdCurrency})."
                : string.Empty;
        }
    }
}