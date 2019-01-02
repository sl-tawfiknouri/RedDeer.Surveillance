using System;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.CancelledOrders.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Rules.CancelledOrders
{
    public class CancelledOrderMessageSender : BaseMessageSender, ICancelledOrderMessageSender
    {
        public CancelledOrderMessageSender(
            ILogger<CancelledOrderMessageSender> logger,
            ICaseMessageSender caseMessageSender)
            : base(
                "Automated Cancellation Ratio Rule Breach Detected",
                "Cancelled Order Message Sender",
                logger,
                caseMessageSender)
        { }

        public async Task Send(ICancelledOrderRuleBreach ruleBreach, ISystemProcessOperationRunRuleContext opCtx)
        {
            if (ruleBreach?.Trades == null
                || !ruleBreach.Trades.Get().Any())
            {
                return;
            }

            var description = BuildDescription(ruleBreach.Parameters, ruleBreach, ruleBreach.Trades.Get().FirstOrDefault());
            await Send(ruleBreach, description, opCtx);
        }

        private string BuildDescription(
            ICancelledOrderRuleParameters parameters,
            ICancelledOrderRuleBreach ruleBreach,
            Order anyOrder)
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

            var positionSizeSegment = PositionSizeText(parameters, ruleBreach, percentagePositionCancelled);
            var orderRatioSegment = OrderRatioText(parameters, ruleBreach, tradeCountCancelled);

            var description = $"Cancelled Order Rule Breach. Traded ({anyOrder?.OrderPosition.GetDescription()}) security {anyOrder?.Instrument?.Name} ({anyOrder?.Instrument?.Identifiers}) with excessive cancellations in {parameters.WindowSize.TotalMinutes} minute time period.{positionSizeSegment}{orderRatioSegment}";

            return description;
        }

        private string PositionSizeText(
            ICancelledOrderRuleParameters parameters,
            ICancelledOrderRuleBreach ruleBreach,
            decimal percentagePositionCancelled)
        {
            return ruleBreach.ExceededPercentagePositionCancellations
                ? $" Position cancelled exceeded threshold at {percentagePositionCancelled}% cancelled. Limit was set at {parameters.CancelledOrderPercentagePositionThreshold * 100}%. {ruleBreach.AmountOfPositionInTotal} orders in the security in during the breach in total of which {ruleBreach.AmountOfPositionCancelled} were cancelled."
                : string.Empty;
        }

        private string OrderRatioText(
            ICancelledOrderRuleParameters parameters,
            ICancelledOrderRuleBreach ruleBreach,
            decimal tradeCountCancelled)
        {
            return ruleBreach.ExceededPercentageTradeCountCancellations
                ? $" Number of orders cancelled exceeded threshold at {tradeCountCancelled}% cancelled. Limit was set at {parameters.CancelledOrderCountPercentageThreshold * 100}%."
                : string.Empty;
        }
    }
}