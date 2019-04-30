using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    public class RampingRuleMessageSender : BaseMessageSender, IRampingRuleMessageSender
    {
        public RampingRuleMessageSender(
            IQueueCasePublisher queueCasePublisher,
            IRuleBreachRepository ruleBreachRepository,
            IRuleBreachOrdersRepository ruleBreachOrdersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper,
            ILogger<IRampingRuleMessageSender> logger) 
            : base(
                "Automated Ramping Rule Breach Detected",
                "Ramping Rule Message Sender",
                logger,
                queueCasePublisher,
                ruleBreachRepository,
                ruleBreachOrdersRepository,
                ruleBreachToRuleBreachOrdersMapper,
                ruleBreachToRuleBreachMapper)
        { }

        public async Task Send(IRampingRuleBreach breach)
        {
            if (breach == null)
            {
                Logger?.LogInformation("Send received a null rule breach for op ctx. Returning.");
                return;
            }

            var description = BuildDescription(breach);
            await Send(breach, description);
        }

        private string BuildDescription(IRampingRuleBreach breach)
        {
            var description = "Ramping Rule Breach.";

            return description;
        }

        public int Flush()
        {
            return -1;
        }
    }
}
