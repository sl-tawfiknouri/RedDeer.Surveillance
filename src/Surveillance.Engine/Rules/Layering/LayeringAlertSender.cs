using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.MessageBusIO.Interfaces;
using Surveillance.Engine.Rules.Rules.Layering.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Layering
{
    public class LayeringAlertSender : BaseMessageSender, ILayeringAlertSender
    {
        public LayeringAlertSender(
            ILogger<LayeringAlertSender> logger,
            ICaseMessageSender caseMessageSender,
            IRuleBreachRepository repository,
            IRuleBreachOrdersRepository ordersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper) 
            : base(
                "Automated Layering Rule Breach Detected",
                "Layering Message Sender",
                logger, 
                caseMessageSender,
                repository,
                ordersRepository,
                ruleBreachToRuleBreachOrdersMapper,
                ruleBreachToRuleBreachMapper)
        { }

        public async Task Send(ILayeringRuleBreach breach)
        {
            if (breach == null)
            {
                Logger.LogInformation($"LayeringAlertSender Send received a null rule breach. Returning.");
                return;
            }

            var description = BuildDescription(breach);
            await Send(breach, description);
        }

        private string BuildDescription(ILayeringRuleBreach breach)
        {
            return $"Layering rule breach detected for {breach.Security.Name} ({breach.Security.Identifiers}).{breach.BidirectionalTradeBreach.Description}{breach.DailyVolumeTradeBreach.Description}{breach.WindowVolumeTradeBreach.Description}{breach.PriceMovementBreach.Description}";
        }
    }
}
