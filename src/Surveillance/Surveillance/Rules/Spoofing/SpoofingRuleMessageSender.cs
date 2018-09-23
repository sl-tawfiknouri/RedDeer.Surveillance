using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Mappers.Interfaces;
using Surveillance.MessageBus_IO.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Rules.Spoofing
{
    public class SpoofingRuleMessageSender : BaseMessageSender, ISpoofingRuleMessageSender
    {

        public SpoofingRuleMessageSender(
            ITradeOrderDataItemDtoMapper dtoMapper,
            ILogger<SpoofingRuleMessageSender> logger,
            ICaseMessageSender caseMessageSender) 
            : base(
                dtoMapper,
                "Automated Spoofing Rule Breach Detected",
                "Spoofing Rule Message Sender",
                logger,
                caseMessageSender)
        { }

        public void Send(ISpoofingRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                return;
            }

            var description = BuildDescription(ruleBreach);
            Send(ruleBreach, description);
        }

        private string BuildDescription(ISpoofingRuleBreach ruleBreach)
        {
            var volumeInPosition = ruleBreach.Trades.VolumeInStatus(OrderStatus.Fulfilled);
            var volumeSpoofed = ruleBreach.CancelledTrades.VolumeNotInStatus(OrderStatus.Fulfilled);

            var description = $"Spoofing Rule Breach. Traded ({ruleBreach.MostRecentTrade.Position.GetDescription()}) security {ruleBreach.Security?.Name} ({ruleBreach.Security?.Identifiers}) with a fulfilled trade volume of {volumeInPosition} and a cancelled trade volume of {volumeSpoofed}. The cancelled volume was traded in the opposite position to the most recent fulfilled trade and is therefore considered to be supporting evidence of spoofing.";

            return description;
        }
    }
}
