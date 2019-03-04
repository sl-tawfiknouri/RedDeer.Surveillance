﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume
{
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
        { }

        public async Task Send(IHighVolumeRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                Logger.LogInformation($"HighVolumeMessageSender send had a null rule breach. Returning");
                return;
            }

            var description = BuildDescription(ruleBreach);
            await Send(ruleBreach, description);
        }

        private string BuildDescription(IHighVolumeRuleBreach ruleBreach)
        {
            var description = $"High Volume rule breach detected for {ruleBreach.Security?.Name} ({ruleBreach.Security?.Identifiers}).";
            var dailyDescription = string.Empty;
            var windowDescription = string.Empty;
            var marketCapDescription = string.Empty;

            if (ruleBreach.DailyBreach.HasBreach)
            {
                var dailyPercentage = Math.Ceiling(ruleBreach.EquitiesParameters.HighVolumePercentageDaily.GetValueOrDefault(0) * 100);
                var dailyBreachPercentage = Math.Ceiling(ruleBreach.DailyBreach.BreachPercentage.GetValueOrDefault(0) * 100);

                dailyDescription = $" Percentage of daily volume breach has occured. A daily volume limit of {dailyPercentage}% was exceeded by trading {dailyBreachPercentage}% of daily volume. {ruleBreach.TotalOrdersTradedInWindow} volume was ordered against a breach threshold volume of {ruleBreach.DailyBreach.BreachThresholdAmount}.";
            }

            if (ruleBreach.WindowBreach.HasBreach)
            {
                var windowPercentage = Math.Ceiling(ruleBreach.EquitiesParameters.HighVolumePercentageWindow.GetValueOrDefault(0) * 100);
                var windowBreachPercentage = Math.Ceiling(ruleBreach.WindowBreach.BreachPercentage.GetValueOrDefault(0) * 100);
                
                windowDescription = $" Percentage of window volume breach has occured. A window volume limit of {windowPercentage}% was exceeded by trading {windowBreachPercentage}% of window volume within the window of {ruleBreach.EquitiesParameters.WindowSize.TotalMinutes} minutes. {ruleBreach.TotalOrdersTradedInWindow} volume was ordered against a breach threshold volume of {ruleBreach.WindowBreach.BreachThresholdAmount}.";
            }

            if (ruleBreach.MarketCapBreach.HasBreach)
            {
                var marketCapPercentage = Math.Ceiling(ruleBreach.EquitiesParameters.HighVolumePercentageMarketCap.GetValueOrDefault(0) * 100);
                var marketCapBreachPercentage = Math.Ceiling(ruleBreach.MarketCapBreach.BreachPercentage.GetValueOrDefault(0) * 100);
                var tradedAmount = Math.Round(ruleBreach.MarketCapBreach.BreachTradedAmountCurrency.Value, 2, MidpointRounding.AwayFromZero);
                var thresholdAmount = Math.Round(ruleBreach.MarketCapBreach.BreachThresholdAmountCurrency.Value, 2, MidpointRounding.AwayFromZero);

                marketCapDescription = $" Percentage of market capitalisation breach has occured. A limit of {marketCapPercentage}% was exceeded by trading {marketCapBreachPercentage}% of market capitalisation.  ({ruleBreach.MarketCapBreach.BreachTradedAmountCurrency.Currency.Value}) {tradedAmount} was traded against a breach threshold value of ({ruleBreach.MarketCapBreach.BreachThresholdAmountCurrency.Currency.Value}) {thresholdAmount}.";
            }

            description = $"{description}{dailyDescription}{windowDescription}{marketCapDescription}";

            return description;
        }
    }
}