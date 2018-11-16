using System;
using Microsoft.Extensions.Logging;
using Surveillance.Mappers.Interfaces;
using Surveillance.MessageBus_IO.Interfaces;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.WashTrade
{
    public class WashTradeRuleMessageSender : BaseMessageSender, IWashTradeRuleMessageSender
    {
        public WashTradeRuleMessageSender(
            ITradeOrderDataItemDtoMapper dtoMapper,
            ILogger logger,
            ICaseMessageSender caseMessageSender) 
            : base(
                dtoMapper,
                "Automated Wash Trade Rule Breach Detected",
                "Wash Trade Rule Message Sender",
                logger,
                caseMessageSender)
        { }

        public void Send(
            IWashTradeRuleBreach breach,
            ISystemProcessOperationRunRuleContext opCtx)
        {
            var description = BuildDescription(breach);
            Send(breach, description, opCtx);
        }

        private string BuildDescription(IWashTradeRuleBreach breach)
        {
            if (breach == null)
            {
                return string.Empty;
            }

            var preamble = $"Wash trade rule breach. Traded ({breach.Security?.Name} ({breach?.Security?.Identifiers.ToString()}).";

            var positionAverage = string.Empty;

            if (breach.AveragePositionBreach?.AveragePositionRuleBreach ?? false)
            {
                positionAverage = BuildAveragePositionDescription(breach);
            }

            return $"{preamble}{positionAverage}";
        }

        private string BuildAveragePositionDescription(IWashTradeRuleBreach breach)
        {
            var trades = breach.AveragePositionBreach.AveragePositionAmountOfTrades.GetValueOrDefault(0);

            var percentageChange =
                Math.Round(
                    breach.AveragePositionBreach.AveragePositionRelativeValueChange.GetValueOrDefault(0) * 100,
                    2,
                    MidpointRounding.AwayFromZero);

            var percentageChangeMax =
                Math.Round(
                    breach.Parameters.AveragePositionMaximumPositionValueChange.GetValueOrDefault(0) * 100,
                    2,
                    MidpointRounding.AwayFromZero);

            var averagePosition = $" {trades} appeared to be part of a series of wash trade activity. These trades netted a total of {percentageChange}% in the value of the traders position, lower values of change are considered to be stronger evidence of wash trading. {percentageChangeMax}% was the configured maximum value change for this to be considered an alert.";

            if (breach.Parameters.AveragePositionMaximumAbsoluteValueChangeAmount != null
                && breach.AveragePositionBreach.AveragePositionAbsoluteValueChange != null)
            {
                var absoluteChange =
                    Math.Round(breach.AveragePositionBreach.AveragePositionAbsoluteValueChange.Value.Value, 2,
                        MidpointRounding.AwayFromZero);

                averagePosition = $"{averagePosition} The absolute value change of the traders position in {breach.Security.Name} changed by ({breach.AveragePositionBreach.AveragePositionAbsoluteValueChange.Value.Currency.Value}){absoluteChange} against a maximum position value change of ({breach.Parameters.AveragePositionMaximumAbsoluteValueChangeCurrency}){breach.Parameters.AveragePositionMaximumAbsoluteValueChangeAmount}.";
            }

            return averagePosition;
        }
    }
}
