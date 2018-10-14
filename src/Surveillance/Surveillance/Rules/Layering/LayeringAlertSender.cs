using Microsoft.Extensions.Logging;
using Surveillance.Mappers.Interfaces;
using Surveillance.MessageBus_IO.Interfaces;
using Surveillance.Rules.Layering.Interfaces;

namespace Surveillance.Rules.Layering
{
    public class LayeringAlertSender : BaseMessageSender, ILayeringAlertSender
    {
        public LayeringAlertSender(
            ITradeOrderDataItemDtoMapper dtoMapper,
            ILogger<LayeringAlertSender> logger,
            ICaseMessageSender caseMessageSender) 
            : base(dtoMapper, "Automated Layering Rule Breach Detected", "Layering Message Sender", logger, caseMessageSender)
        { }

        public void Send(ILayeringRuleBreach breach)
        {
            if (breach == null)
            {
                return;
            }

            var description = BuildDescription(breach);
            Send(breach, description);
        }

        private string BuildDescription(ILayeringRuleBreach breach)
        {
            var bidirectionalTradeDescription =
                breach.BidirectionalTradeBreach
                    ? " Trading in both buy/sell positions simultaneously was detected."
                    : string.Empty;

            var dailyVolumeTradeDescription =
                breach.DailyVolumeTradeBreach
                    ? $" Trading in a layered position exceeded the daily volume threshold set at {breach.Parameters.PercentageOfMarketDailyVolume * 100}%."
                    : string.Empty;

            var windowVolumeTradeDescription =
                breach.WindowVolumeTradeBreach
                    ? $" Trading in a layered position exceed the window volume threshold set at {breach.Parameters.PercentageOfMarketWindowVolume * 100}%."
                    : string.Empty;

            var priceMovementTradeDescription =
                breach.PriceMovementBreach
                    ? $" Trading in a layered position failed the price movement check."
                    : string.Empty;

            return $"Layering rule breach detected for {breach.Security.Name} ({breach.Security.Identifiers}).{bidirectionalTradeDescription}{dailyVolumeTradeDescription}{windowVolumeTradeDescription}{priceMovementTradeDescription}";
        }
    }
}
