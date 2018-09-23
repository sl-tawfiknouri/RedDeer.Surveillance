﻿using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Mappers.Interfaces;
using Surveillance.MessageBus_IO.Interfaces;
using Surveillance.Rules.Cancelled_Orders.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Rules.Cancelled_Orders
{
    public class CancelledOrderMessageSender : BaseMessageSender, ICancelledOrderMessageSender
    {
        public CancelledOrderMessageSender(
            ITradeOrderDataItemDtoMapper dtoMapper,
            ILogger<CancelledOrderMessageSender> logger,
            ICaseMessageSender caseMessageSender)
            : base(
                dtoMapper,
                "Automated Cancellation Ratio Rule Breach Detected",
                "Cancelled Order Message Sender",
                logger,
                caseMessageSender)
        { }

        public void Send(ICancelledOrderRuleBreach ruleBreach)
        {
            if (ruleBreach?.Trades == null
                || !ruleBreach.Trades.Get().Any())
            {
                return;
            }

            var description = BuildDescription(ruleBreach.Parameters, ruleBreach, ruleBreach.Trades.Get().FirstOrDefault());
            Send(ruleBreach, description);
        }

        private string BuildDescription(
            ICancelledOrderRuleParameters parameters,
            ICancelledOrderRuleBreach ruleBreach,
            Domain.Trades.Orders.TradeOrderFrame anyOrder)
        {
            var percentagePositionCancelled =
                Math.Round(
                    (ruleBreach.PercentagePositionCancelled.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var tradeCountCancelled =
                Math.Round(
                    (ruleBreach.PercentageTradeCountCancelled.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);


            var positionSizeSegment = ruleBreach.ExceededPercentagePositionCancellations
                ? $" Position cancelled exceeded threshold at {percentagePositionCancelled}% cancelled. Limit was set at {parameters.CancelledOrderPercentagePositionThreshold * 100}%."
                : string.Empty;

            var orderRatioSegment = ruleBreach.ExceededPercentageTradeCountCancellations
                ? $" Number of orders cancelled exceeded threshold at {tradeCountCancelled}% cancelled. Limit was set at {parameters.CancelledOrderCountPercentageThreshold * 100}%."
                : string.Empty;

            var description = $"Cancelled Order Rule Breach. Traded ({anyOrder?.Position.GetDescription()}) security {anyOrder?.Security?.Name} ({anyOrder?.Security?.Identifiers}) with excessive cancellations in {parameters.WindowSize.TotalMinutes} minute time period.{positionSizeSegment}{orderRatioSegment}";

            return description;
        }
    }
}