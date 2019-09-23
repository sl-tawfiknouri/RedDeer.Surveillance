namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Rules.Interfaces;
    using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;

    public class HighVolumeMessageSender : BaseMessageSender, IHighVolumeMessageSender
    {
        public HighVolumeMessageSender(
            ILogger<IHighVolumeMessageSender> logger,
            IQueueCasePublisher queueCasePublisher,
            IRuleBreachRepository repository,
            IRuleBreachOrdersRepository ordersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper)
            : base(
                "Automated High Volume Rule Breach Detected",
                "High Volume Message Sender",
                logger,
                queueCasePublisher,
                repository,
                ordersRepository,
                ruleBreachToRuleBreachOrdersMapper,
                ruleBreachToRuleBreachMapper)
        {
        }

        public async Task Send(IHighVolumeRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                this.Logger.LogInformation("HighVolumeMessageSender send had a null rule breach. Returning");
                return;
            }

            var description = this.BuildDescription(ruleBreach);
            await this.Send(ruleBreach, description);
        }

        public string BuildDescription(IHighVolumeRuleBreach ruleBreach)
        {
            var description = $"High Volume rule breach detected for {ruleBreach.Security?.Name}.";
            var venueDailyDescription =
                ruleBreach.DailyBreach.Venue != null 
                ? $" at the venue ({ruleBreach.DailyBreach.Venue?.MarketIdentifierCode}) {ruleBreach.DailyBreach.Venue?.Name}"
                : string.Empty;
            var venueWindowDescription =
                ruleBreach.WindowBreach.Venue != null
                    ? $" at the venue ({ruleBreach.WindowBreach.Venue?.MarketIdentifierCode}) {ruleBreach.WindowBreach.Venue?.Name}"
                    : string.Empty;
            var dailyDescription = string.Empty;
            var windowDescription = string.Empty;
            var marketCapDescription = string.Empty;

            if (ruleBreach.DailyBreach.HasBreach)
            {
                var dailyPercentage = Math.Ceiling(
                    ruleBreach.EquitiesParameters.HighVolumePercentageDaily.GetValueOrDefault(0) * 100);
                var dailyBreachPercentage =
                    Math.Ceiling(ruleBreach.DailyBreach.BreachPercentage.GetValueOrDefault(0) * 100);
                
                dailyDescription = $" Percentage of daily volume breach has occured. A daily volume limit of {dailyPercentage.ToString("0.##")}% was exceeded by trading {dailyBreachPercentage.ToString("0.##")}% of daily volume{venueDailyDescription}. {ruleBreach.TotalOrdersTradedInWindow.ToString("0.##")} volume was the allocated fill against a breach threshold volume of {ruleBreach.DailyBreach.BreachThresholdAmount.ToString("0.##")}.";
            }

            if (ruleBreach.WindowBreach.HasBreach)
            {
                var windowPercentage = Math.Ceiling(
                    ruleBreach.EquitiesParameters.HighVolumePercentageWindow.GetValueOrDefault(0) * 100);
                var windowBreachPercentage =
                    Math.Ceiling(ruleBreach.WindowBreach.BreachPercentage.GetValueOrDefault(0) * 100);

                windowDescription = $" Percentage of window volume breach has occured. A window volume limit of {windowPercentage.ToString("0.##")}% was exceeded by trading {windowBreachPercentage.ToString("0.##")}% of window volume within the window of {ruleBreach.EquitiesParameters.Windows.BackwardWindowSize.TotalMinutes} minutes{venueWindowDescription}. {ruleBreach.TotalOrdersTradedInWindow.ToString("0.##")} volume was the allocated fill against a breach threshold volume of {ruleBreach.WindowBreach.BreachThresholdAmount.ToString("0.##")}.";
            }

            if (ruleBreach.MarketCapBreach.HasBreach)
            {
                var marketCapPercentage = Math.Ceiling(
                    ruleBreach.EquitiesParameters.HighVolumePercentageMarketCap.GetValueOrDefault(0) * 100);
                var marketCapBreachPercentage =
                    Math.Ceiling(ruleBreach.MarketCapBreach.BreachPercentage.GetValueOrDefault(0) * 100);
                var tradedAmount = Math.Round(
                    ruleBreach.MarketCapBreach.BreachTradedMoney.Value,
                    2,
                    MidpointRounding.AwayFromZero);
                var currencyCode =
                    string.IsNullOrWhiteSpace(ruleBreach.MarketCapBreach.BreachTradedMoney.Currency.Code)
                    ? string.Empty
                    : $"({ruleBreach.MarketCapBreach.BreachTradedMoney.Currency.Code})";
                var thresholdAmount = Math.Round(
                    ruleBreach.MarketCapBreach.BreachThresholdMoney.Value,
                    2,
                    MidpointRounding.AwayFromZero);

                marketCapDescription = $" Percentage of market capitalisation breach has occured. A limit of {marketCapPercentage.ToString("0.##")}% was exceeded by trading {marketCapBreachPercentage.ToString("0.##")}% of market capitalisation.  {currencyCode} {tradedAmount.ToString("0.##")} was traded against a breach threshold value of {currencyCode} {thresholdAmount.ToString("0.##")}.";
            }

            description = $"{description}{dailyDescription}{windowDescription}{marketCapDescription}";

            return description;
        }
    }
}