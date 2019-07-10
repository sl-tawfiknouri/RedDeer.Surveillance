using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    public class HighProfitMessageSender : BaseMessageSender, IHighProfitMessageSender
    {
        public HighProfitMessageSender(
            ILogger<HighProfitMessageSender> logger,
            IQueueCasePublisher queueCasePublisher,
            IRuleBreachRepository repository,
            IRuleBreachOrdersRepository ordersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper)
            : base(
                "Automated High Profit Rule Breach Detected",
                "High Profit Message Sender",
                logger,
                queueCasePublisher,
                repository,
                ordersRepository,
                ruleBreachToRuleBreachOrdersMapper,
                ruleBreachToRuleBreachMapper)
        { }

        public async Task Send(IHighProfitRuleBreach ruleBreach)
        {
            var description = BuildDescription(ruleBreach);
            await Send(ruleBreach, description);
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
                    (ruleBreach.EquitiesParameters.HighProfitPercentageThreshold.GetValueOrDefault(0) * 100m),
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
            
            return $"High profit rule breach detected for {ruleBreach.Security.Name} at {ruleBreach.UniverseDateTime}.{highRelativeProfitSection}{highAbsoluteProfitSection}{highProfitExchangeRatesSection}";
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
                ? $" There was a high profit of {absoluteProfit} ({ruleBreach.AbsoluteProfitCurrency}) which exceeded the configured profit limit of {ruleBreach.EquitiesParameters.HighProfitAbsoluteThreshold.GetValueOrDefault(0)}({ruleBreach.EquitiesParameters.HighProfitCurrencyConversionTargetCurrency})."
                : string.Empty;
        }

        private string HighProfitExchangeRateText(IHighProfitRuleBreach ruleBreach)
        {
            if (!ruleBreach.EquitiesParameters.UseCurrencyConversions
                || ruleBreach.ExchangeRateProfits == null)
            {
                return string.Empty;
            }

            if (string.Equals(
                ruleBreach.ExchangeRateProfits.FixedCurrency.Code,
                ruleBreach.ExchangeRateProfits.VariableCurrency.Code,
                StringComparison.InvariantCultureIgnoreCase))
            {
                Logger.LogError($"HighProfitMessageSender had two equal currencies when generating WER text {ruleBreach.ExchangeRateProfits.FixedCurrency.Code} and {ruleBreach.ExchangeRateProfits.VariableCurrency.Code}");
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

            return $" The position was acquired with a currency conversion between ({ruleBreach.ExchangeRateProfits.FixedCurrency.Code}/{ruleBreach.ExchangeRateProfits.VariableCurrency.Code}) rate at a weighted exchange rate of {costWer} and sold at a weighted exchange rate of {revenueWer}. The impact on profits from exchange rate movements was {relativePercentage}% and the absolute amount of profits due to exchange rates is ({ruleBreach.AbsoluteProfitCurrency}) {absAmount}.";
        }
    }
}