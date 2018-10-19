using System;
using Microsoft.Extensions.Logging;
using Surveillance.Mappers.Interfaces;
using Surveillance.MessageBus_IO.Interfaces;
using Surveillance.Rules.HighVolume.Interfaces;

namespace Surveillance.Rules.HighVolume
{
    public class HighVolumeMessageSender : BaseMessageSender, IHighVolumeMessageSender
    {
        public HighVolumeMessageSender(
            ITradeOrderDataItemDtoMapper dtoMapper,
            ILogger<IHighVolumeMessageSender> logger,
            ICaseMessageSender caseMessageSender)
            : base(
                dtoMapper,
                "Automated High Volume Rule Breach Detected",
                "High Volume Message Sender",
                logger,
                caseMessageSender)
        { }

        public void Send(IHighVolumeRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                return;
            }

            var description = BuildDescription(ruleBreach);
            Send(ruleBreach, description);
        }

        private string BuildDescription(IHighVolumeRuleBreach ruleBreach)
        {
            var description = $"High Volume rule breach detected for {ruleBreach.Security?.Name} ({ruleBreach.Security?.Identifiers}).";

            if (ruleBreach.DailyBreach.HasBreach)
            {
                var dailyPercentage = Math.Ceiling(ruleBreach.Parameters.HighVolumePercentageDaily.GetValueOrDefault(0) * 100);
                var dailyBreachPercentage = Math.Ceiling(ruleBreach.DailyBreach.BreachPercentage.GetValueOrDefault(0) * 100);

                var dailyDescription = $" Percentage of daily volume breach has occured. A daily volume limit of {dailyPercentage}% was exceeded by trading {dailyBreachPercentage}% of daily volume. {ruleBreach.TotalOrdersTradedInWindow} orders were submitted against a breach threshold volume of {ruleBreach.DailyBreach.BreachThresholdAmount}.";

                description = $"{description}{dailyDescription}";
            }

            if (ruleBreach.WindowBreach.HasBreach)
            {
                var windowPercentage = Math.Ceiling(ruleBreach.Parameters.HighVolumePercentageWindow.GetValueOrDefault(0) * 100);
                var windowBreachPercentage = Math.Ceiling(ruleBreach.WindowBreach.BreachPercentage.GetValueOrDefault(0) * 100);
                
                var windowDescription = $" Percentage of window volume breach has occured. A window volume limit of {windowPercentage}% was exceeded by trading {windowBreachPercentage}% of window volume within the window of {ruleBreach.Parameters.WindowSize.TotalMinutes} minutes. {ruleBreach.TotalOrdersTradedInWindow} orders were submitted against a breach threshold volume of {ruleBreach.WindowBreach.BreachThresholdAmount}.";

                description = $"{description}{windowDescription}";
            }

            return description;
        }
    }
}
