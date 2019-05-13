using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;
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
            ILogger logger) 
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

        public async Task Send(IPlacingOrdersWithNoIntentToExecuteRuleBreach ruleRuleBreach)
        {
            if (ruleRuleBreach == null)
            {
                Logger?.LogInformation($"SpoofingRuleMessageSender Send received a null rule breach for op ctx. Returning.");
                return;
            }
        }
    }
}
