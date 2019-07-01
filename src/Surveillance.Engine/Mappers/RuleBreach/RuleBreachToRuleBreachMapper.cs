using System;
using System.Linq;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Mappers.RuleBreach
{
    public class RuleBreachToRuleBreachMapper : IRuleBreachToRuleBreachMapper
    {
        public Domain.Surveillance.Rules.RuleBreach RuleBreachItem(IRuleBreach ruleBreach, string description, string caseTitle)
        {
            var oldestPosition = ruleBreach.Trades?.Get()?.Min(tr => tr.MostRecentDateEvent());
            var latestPosition = ruleBreach.Trades?.Get()?.Max(tr => tr.MostRecentDateEvent());
            var venue = ruleBreach.Trades?.Get()?.FirstOrDefault()?.Market?.Name;

            if (oldestPosition == null)
            {
                oldestPosition = latestPosition;
            }

            if (latestPosition == null)
            {
                latestPosition = oldestPosition;
            }

            var oldestPositionValue = oldestPosition ?? DateTime.UtcNow;
            var latestPositionValue = latestPosition ?? DateTime.UtcNow;

            description = description ?? string.Empty;

            var trades =
                ruleBreach
                    .Trades
                    ?.Get()
                    ?.Select(io => io.ReddeerOrderId)
                    .Where(x => x.HasValue)
                    .Select(y => y.Value)
                    .ToList();

            var ruleBreachObj =
                new Domain.Surveillance.Rules.RuleBreach(
                    null,
                    ruleBreach.RuleParameterId,
                    ruleBreach.CorrelationId,
                    ruleBreach.IsBackTestRun,
                    DateTime.UtcNow,
                    caseTitle,
                    description,
                    venue,
                    oldestPositionValue,
                    latestPositionValue,
                    ruleBreach.Security.Cfi,
                    ruleBreach.Security.Identifiers.ReddeerEnrichmentId,
                    ruleBreach.SystemOperationId,
                    (int?)ruleBreach?.FactorValue?.OrganisationalFactors ?? 0,
                    ruleBreach?.FactorValue?.Value ?? string.Empty,
                    ruleBreach.RuleParameters.TunedParam != null,
                    trades);

            return ruleBreachObj;
        }
    }
}
