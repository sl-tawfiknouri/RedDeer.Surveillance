using System.Threading.Tasks;
using Domain.Enums;
using Domain.Financial;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.Rules.Spoofing.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Spoofing
{
    public class SpoofingRuleMessageSender : BaseMessageSender, ISpoofingRuleMessageSender
    {

        public SpoofingRuleMessageSender(
            ILogger<SpoofingRuleMessageSender> logger,
            IQueueCasePublisher queueCasePublisher,
            IRuleBreachRepository repository,
            IRuleBreachOrdersRepository ordersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper) 
            : base(
                "Automated Spoofing Rule Breach Detected",
                "Spoofing Rule Message Sender",
                logger,
                queueCasePublisher,
                repository,
                ordersRepository,
                ruleBreachToRuleBreachOrdersMapper,
                ruleBreachToRuleBreachMapper)
        { }

        public async Task Send(ISpoofingRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                Logger?.LogInformation($"SpoofingRuleMessageSender Send received a null rule breach for op ctx. Returning.");
                return;
            }

            var description = BuildDescription(ruleBreach);
            await Send(ruleBreach, description);
        }

        private string BuildDescription(ISpoofingRuleBreach ruleBreach)
        {
            var volumeInPosition = ruleBreach.Trades.VolumeInStatus(OrderStatus.Filled);
            var volumeSpoofed = ruleBreach.CancelledTrades.VolumeNotInStatus(OrderStatus.Filled);

            var description = $"Spoofing Rule Breach. Traded ({ruleBreach.MostRecentTrade.OrderDirection.GetDescription()}) security {ruleBreach.Security?.Name} ({ruleBreach.Security?.Identifiers}) with a fulfilled trade volume of {volumeInPosition} and a cancelled trade volume of {volumeSpoofed}. The cancelled volume was traded in the opposite position to the most recent fulfilled trade and is therefore considered to be supporting evidence of spoofing.";

            return description;
        }
    }
}
