﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits
{
    public class HighProfitMessageSender : BaseMessageSender, IHighProfitMessageSender
    {
        public HighProfitMessageSender(
            ILogger<HighProfitMessageSender> logger,
            ICaseMessageSender caseMessageSender)
            : base(
                "Automated High Profit Rule Breach Detected",
                "High Profit Message Sender",
                logger,
                caseMessageSender)
        { }

        public async Task Send(IHighProfitRuleBreach ruleBreach, ISystemProcessOperationRunRuleContext opCtx)
        {
            var description = BuildDescription(ruleBreach);
            await Send(ruleBreach, description, opCtx);
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

            var highAbsoluteProfit =
                Math.Round(
                    (ruleBreach.AbsoluteProfits.GetValueOrDefault(0)),
                    2,
                    MidpointRounding.AwayFromZero);

            var highRelativeProfitSection =
                HighRelativeProfitText(ruleBreach, highRelativeProfitAsPercentage, highRelativeProfitAsPercentageSetByUser);

            var highAbsoluteProfitSection = HighAbsoluteProfitText(ruleBreach, highAbsoluteProfit);
            var highProfitExchangeRatesSection = HighProfitExchangeRateText(ruleBreach);

            return $"High profit rule breach detected for {ruleBreach.Security.Name} ({ruleBreach.Security.Identifiers}).{highRelativeProfitSection}{highAbsoluteProfitSection}{highProfitExchangeRatesSection}";
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

        private string HighAbsoluteProfitText(IHighProfitRuleBreach ruleBreach, decimal absoluteProfit)
        {
            return ruleBreach.HasAbsoluteProfitBreach
                ? $" There was a high profit of {absoluteProfit} ({ruleBreach.AbsoluteProfitCurrency}) which exceeded the configured profit limit of {ruleBreach.Parameters.HighProfitAbsoluteThreshold.GetValueOrDefault(0)}({ruleBreach.Parameters.HighProfitCurrencyConversionTargetCurrency})."
                : string.Empty;
        }

        private string HighProfitExchangeRateText(IHighProfitRuleBreach ruleBreach)
        {
            if (!ruleBreach.Parameters.UseCurrencyConversions
                || ruleBreach.ExchangeRateProfits == null)
            {
                return string.Empty;
            }

            if (string.Equals(
                ruleBreach.ExchangeRateProfits.FixedCurrency.Value,
                ruleBreach.ExchangeRateProfits.VariableCurrency.Value,
                StringComparison.InvariantCultureIgnoreCase))
            {
                Logger.LogError($"HighProfitMessageSender had two equal currencies when generating WER text {ruleBreach.ExchangeRateProfits.FixedCurrency.Value} and {ruleBreach.ExchangeRateProfits.VariableCurrency.Value}");
                return string.Empty;
            }

            var absAmount = Math.Round(
                ruleBreach.ExchangeRateProfits.AbsoluteAmountDueToWer(),
                2,
                MidpointRounding.AwayFromZero);

            var costWer = Math.Round(
                ruleBreach.ExchangeRateProfits.PositionCostWer,
                2,
                MidpointRounding.AwayFromZero);

            var revenueWer = Math.Round(
                ruleBreach.ExchangeRateProfits.PositionRevenueWer,
                2,
                MidpointRounding.AwayFromZero);

            var relativePercentage = Math.Round(
                ruleBreach.ExchangeRateProfits.RelativePercentageDueToWer() * 100,
                2,
                MidpointRounding.AwayFromZero);

            if (revenueWer == 0
                || costWer == 0)
            {
                return string.Empty;
            }

            return $" The position was acquired with a currency conversion between ({ruleBreach.ExchangeRateProfits.FixedCurrency.Value}/{ruleBreach.ExchangeRateProfits.VariableCurrency.Value}) rate at a weighted exchange rate of {costWer} and sold at a weighted exchange rate of {revenueWer}. The impact on profits from exchange rate movements was {relativePercentage}% and the absolute amount of profits due to exchange rates is ({ruleBreach.AbsoluteProfitCurrency}) {absAmount}.";
        }
    }
}