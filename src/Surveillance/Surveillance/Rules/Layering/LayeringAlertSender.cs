using Microsoft.Extensions.Logging;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.Layering
{
    public class LayeringAlertSender : BaseMessageSender, ILayeringAlertSender
    {
        public LayeringAlertSender(
            ILogger<LayeringAlertSender> logger,
            ICaseMessageSender caseMessageSender) 
            : base("Automated Layering Rule Breach Detected", "Layering Message Sender", logger, caseMessageSender)
        { }

        public void Send(ILayeringRuleBreach breach, ISystemProcessOperationRunRuleContext opCtx)
        {
            if (breach == null)
            {
                return;
            }

            var description = BuildDescription(breach);
            Send(breach, description, opCtx);
        }

        private string BuildDescription(ILayeringRuleBreach breach)
        {
            return $"Layering rule breach detected for {breach.Security.Name} ({breach.Security.Identifiers}).{breach.BidirectionalTradeBreach.Description}{breach.DailyVolumeTradeBreach.Description}{breach.WindowVolumeTradeBreach.Description}{breach.PriceMovementBreach.Description}";
        }
    }
}
