﻿namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Rules.Interfaces;
    using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;

    public class WashTradeRuleMessageSender : BaseMessageSender, IWashTradeRuleMessageSender
    {
        public WashTradeRuleMessageSender(
            ILogger<WashTradeRuleMessageSender> logger,
            IQueueCasePublisher queueCasePublisher,
            IRuleBreachRepository repository,
            IRuleBreachOrdersRepository ordersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper)
            : base(
                "Automated Wash Trade Rule Breach Detected",
                "Wash Trade Rule Message Sender",
                logger,
                queueCasePublisher,
                repository,
                ordersRepository,
                ruleBreachToRuleBreachOrdersMapper,
                ruleBreachToRuleBreachMapper)
        {
        }

        public async Task Send(IWashTradeRuleBreach breach)
        {
            var description = this.BuildDescription(breach);
            await this.Send(breach, description);
        }

        private string BuildAveragePositionDescription(IWashTradeRuleBreach breach)
        {
            var trades = breach.AveragePositionBreach.AveragePositionAmountOfTrades.GetValueOrDefault(0);

            var percentageChange = Math.Round(
                breach.AveragePositionBreach.AveragePositionRelativeValueChange.GetValueOrDefault(0) * 100,
                2,
                MidpointRounding.AwayFromZero);

            var percentageChangeMax = Math.Round(
                breach.EquitiesParameters.AveragePositionMaximumPositionValueChange.GetValueOrDefault(0) * 100,
                2,
                MidpointRounding.AwayFromZero);

            var averagePosition =
                $" {trades} trades appeared to be part of a series of wash trade activity. These trades netted a total of {percentageChange}% in the value of the traders position, lower values of change are considered to be stronger evidence of wash trading. {percentageChangeMax}% was the configured maximum value change for this to be considered an alert.";

            if (breach.EquitiesParameters.AveragePositionMaximumAbsoluteValueChangeAmount != null
                && breach.AveragePositionBreach.AveragePositionAbsoluteValueChange != null)
            {
                var absoluteChange = Math.Round(
                    breach.AveragePositionBreach.AveragePositionAbsoluteValueChange.Value.Value,
                    2,
                    MidpointRounding.AwayFromZero);

                averagePosition =
                    $"{averagePosition} The absolute value change of the traders position in {breach.Security.Name} changed by ({breach.AveragePositionBreach.AveragePositionAbsoluteValueChange.Value.Currency.Code}){absoluteChange} against a maximum position value change of ({breach.EquitiesParameters.AveragePositionMaximumAbsoluteValueChangeCurrency}){breach.EquitiesParameters.AveragePositionMaximumAbsoluteValueChangeAmount}.";
            }

            return averagePosition;
        }

        private string BuildDescription(IWashTradeRuleBreach breach)
        {
            if (breach == null) return string.Empty;

            var preamble = $"Wash trade rule breach. Traded {breach.Security?.Name}.";

            var positionAverage = string.Empty;
            var positionPaired = string.Empty;
            var positionClustering = string.Empty;

            if (breach.AveragePositionBreach != null && breach.AveragePositionBreach.AveragePositionRuleBreach)
                positionAverage = this.BuildAveragePositionDescription(breach);

            if (breach.ClusteringPositionBreach != null && breach.ClusteringPositionBreach.ClusteringPositionBreach)
                positionClustering = this.BuildPositionClusteringDescription(breach);

            return $"{preamble}{positionAverage}{positionPaired}{positionClustering}";
        }

        private string BuildPositionClusteringDescription(IWashTradeRuleBreach breach)
        {
            var percentageChangeMax = Math.Round(
                breach.EquitiesParameters.ClusteringPercentageValueDifferenceThreshold.GetValueOrDefault(0) * 100,
                2,
                MidpointRounding.AwayFromZero);

            var centroids =
                breach.ClusteringPositionBreach?.CentroidsOfBreachingClusters.Select(
                    ce => Math.Round(ce, 2, MidpointRounding.AwayFromZero)) ?? new List<decimal>();

            var initial =
                $" A clustering (k-means) rule breach was found with {breach.ClusteringPositionBreach.AmountOfBreachingClusters} clusters detected to be trading at thin margins per cluster defined as less than a {percentageChangeMax}% difference between cost and revenues when buying and selling a position.";

            if (breach.ClusteringPositionBreach != null
                && breach.ClusteringPositionBreach.CentroidsOfBreachingClusters.Any())
                initial =
                    $"{initial} The centroids of the clusters were at the prices {centroids.Aggregate(string.Empty, (a, b) => $"{a}{b}")}";

            return initial;
        }
    }
}