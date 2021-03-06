﻿namespace Surveillance.Engine.Rules.Rules.Equity.CancelledOrders
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Trading.Orders;

    using Infrastructure.Network.Extensions;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Rules.Interfaces;
    using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;

    public class CancelledOrderMessageSender : BaseMessageSender, ICancelledOrderMessageSender
    {
        public CancelledOrderMessageSender(
            ILogger<CancelledOrderMessageSender> logger,
            IQueueCasePublisher queueCasePublisher,
            IRuleBreachRepository repository,
            IRuleBreachOrdersRepository ordersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper)
            : base(
                "Automated Cancellation Ratio Rule Breach Detected",
                "Cancelled Order Message Sender",
                logger,
                queueCasePublisher,
                repository,
                ordersRepository,
                ruleBreachToRuleBreachOrdersMapper,
                ruleBreachToRuleBreachMapper)
        {
        }

        public async Task Send(ICancelledOrderRuleBreach ruleBreach)
        {
            if (ruleBreach?.Trades == null || !ruleBreach.Trades.Get().Any()) return;

            var description = this.BuildDescription(
                ruleBreach.Parameters,
                ruleBreach,
                ruleBreach.Trades.Get().FirstOrDefault());
            await this.Send(ruleBreach, description);
        }

        private string BuildDescription(
            ICancelledOrderRuleEquitiesParameters parameters,
            ICancelledOrderRuleBreach ruleBreach,
            Order anyOrder)
        {
            var percentagePositionCancelled = Math.Round(
                ruleBreach.PercentagePositionCancelled.GetValueOrDefault(0) * 100m,
                2,
                MidpointRounding.AwayFromZero);

            var tradeCountCancelled = Math.Round(
                ruleBreach.PercentageTradeCountCancelled.GetValueOrDefault(0) * 100m,
                2,
                MidpointRounding.AwayFromZero);

            var positionSizeSegment = this.PositionSizeText(parameters, ruleBreach, percentagePositionCancelled);
            var orderRatioSegment = this.OrderRatioText(parameters, ruleBreach, tradeCountCancelled);

            var description =
                $"Cancelled Order Rule Breach. Traded ({anyOrder?.OrderDirection.GetDescription()}) security {anyOrder?.Instrument?.Name} with excessive cancellations in {parameters.Windows.BackwardWindowSize.TotalMinutes} minute time period.{positionSizeSegment}{orderRatioSegment}";

            return description;
        }

        private string OrderRatioText(
            ICancelledOrderRuleEquitiesParameters parameters,
            ICancelledOrderRuleBreach ruleBreach,
            decimal tradeCountCancelled)
        {
            return ruleBreach.ExceededPercentageTradeCountCancellations
                       ? $" Number of orders cancelled exceeded threshold at {tradeCountCancelled}% cancelled. Limit was set at {parameters.CancelledOrderCountPercentageThreshold * 100}%."
                       : string.Empty;
        }

        private string PositionSizeText(
            ICancelledOrderRuleEquitiesParameters parameters,
            ICancelledOrderRuleBreach ruleBreach,
            decimal percentagePositionCancelled)
        {
            return ruleBreach.ExceededPercentagePositionCancellations
                       ? $" Position cancelled exceeded threshold at {percentagePositionCancelled}% cancelled. Limit was set at {parameters.CancelledOrderPercentagePositionThreshold * 100}%. {ruleBreach.AmountOfPositionInTotal} orders in the security in during the breach in total of which {ruleBreach.AmountOfPositionCancelled} were cancelled."
                       : string.Empty;
        }
    }
}