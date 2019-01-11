using System.Threading.Tasks;
using DomainV2.Financial;
using Microsoft.Extensions.Logging;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Rules.Spoofing
{
    public class SpoofingRuleMessageSender : BaseMessageSender, ISpoofingRuleMessageSender
    {

        public SpoofingRuleMessageSender(
            ILogger<SpoofingRuleMessageSender> logger,
            ICaseMessageSender caseMessageSender) 
            : base(
                "Automated Spoofing Rule Breach Detected",
                "Spoofing Rule Message Sender",
                logger,
                caseMessageSender)
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

            var description = $"Spoofing Rule Breach. Traded ({ruleBreach.MostRecentTrade.OrderPosition.GetDescription()}) security {ruleBreach.Security?.Name} ({ruleBreach.Security?.Identifiers}) with a fulfilled trade volume of {volumeInPosition} and a cancelled trade volume of {volumeSpoofed}. The cancelled volume was traded in the opposite position to the most recent fulfilled trade and is therefore considered to be supporting evidence of spoofing.";

            return description;
        }
    }
}
