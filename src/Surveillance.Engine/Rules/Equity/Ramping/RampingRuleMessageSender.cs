using System.Threading.Tasks;
using Domain.Core.Extensions;
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
            var description = $"Ramping rule breach detected for {breach.Security.Name}. {RampingStrategySummary(breach.SummaryPanel.OneDayAnalysis)}{RampingStrategySummary(breach.SummaryPanel.FiveDayAnalysis)}{RampingStrategySummary(breach.SummaryPanel.FifteenDayAnalysis)}{RampingStrategySummary(breach.SummaryPanel.ThirtyDayAnalysis)}";
            
            return description;
        }

        private string RampingStrategySummary(IRampingStrategySummary summary)
        {
            if (summary == null
                || summary.RampingStrategy == RampingStrategy.Unknown)
            {
                return string.Empty;
            }
            
            return $"{summary.TimeSegment.GetDescription()} ramping analysis detected market prices {summary.TrendClassification.Trend.GetDescription()} trend concurrent with a trading pattern with a {summary.PriceImpact.Classification.GetDescription().ToLower()} price impact. Trading strategy is judged to be {summary.RampingStrategy.GetDescription()} ramping. ";
        }

        public int Flush()
        {
            return -1;
        }
    }
}
