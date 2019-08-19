using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute
{
    public class PlacingOrdersWithNoIntentToExecuteMessageSender : BaseMessageSender, IPlacingOrdersWithNoIntentToExecuteMessageSender
    {
        public PlacingOrdersWithNoIntentToExecuteMessageSender(
            IQueueCasePublisher queueCasePublisher,
            IRuleBreachRepository ruleBreachRepository,
            IRuleBreachOrdersRepository ruleBreachOrdersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper,
            ILogger<PlacingOrdersWithNoIntentToExecuteMessageSender> logger) 
            : base(
                "Automated Placing Orders With No Intent To Execute Rule Breach Detected",
                "Placing Orders Without Intent To Execute Message Sender",
                logger,
                queueCasePublisher,
                ruleBreachRepository,
                ruleBreachOrdersRepository, 
                ruleBreachToRuleBreachOrdersMapper, 
                ruleBreachToRuleBreachMapper)
        {
        }

        public async Task Send(IPlacingOrdersWithNoIntentToExecuteRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                Logger?.LogInformation($"SpoofingRuleMessageSender Send received a null rule breach for op ctx. Returning.");
                return;
            }

            var description = BuildDescription(ruleBreach);
            await Send(ruleBreach, description);
        }

        private string BuildDescription(IPlacingOrdersWithNoIntentToExecuteRuleBreach ruleBreach)
        {
            var sdBreakdown = 
                ruleBreach
                    ?.ProbabilityForOrders
                    ?.Select(_ => _.Sd)
                    .GroupBy(_ => (int) _)
                    .Select(_ => new StandardDeviationCount(_.Count(), _.Key))
                    .ToList()
                ?? new List<StandardDeviationCount>();

            var sdBreakdownText =
                !sdBreakdown.Any()
                    ? string.Empty 
                    : sdBreakdown.Aggregate(string.Empty, (a,b) => $"{a} SD-{b.StandardDeviation} Trades-{b.Count}");

            var fullSdText = 
                !sdBreakdown.Any() 
                    ? string.Empty 
                    : $"The distribution of trades by standard deviation from the mean was {sdBreakdownText}.";

            var description = $"Placing Orders With No Intent To Execute Rule Breach. Traded {ruleBreach?.Trades?.Get()?.Count} trades separately over configured sigma {ruleBreach?.Parameters?.Sigma} probability of reaching their limit price. The standard deviation for the trading day was {ruleBreach?.StandardDeviationPrice} and the mean price was {ruleBreach?.MeanPrice}. {fullSdText}";
            
            return description;
        }

        private class StandardDeviationCount
        {
            public StandardDeviationCount(int count, decimal standardDeviation)
            {
                Count = count;
                StandardDeviation = standardDeviation;
            }

            public int Count { get; }
            public decimal StandardDeviation { get; }
        }
    }
}
