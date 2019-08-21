namespace Surveillance.Engine.Rules.Mappers.RuleBreach
{
    using System;
    using System.Linq;

    using Domain.Surveillance.Rules;

    using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public class RuleBreachToRuleBreachMapper : IRuleBreachToRuleBreachMapper
    {
        public RuleBreach RuleBreachItem(IRuleBreach ruleBreach)
        {
            var oldestPosition = ruleBreach.Trades?.Get()?.Min(tr => tr.MostRecentDateEvent());
            var latestPosition = ruleBreach.Trades?.Get()?.Max(tr => tr.MostRecentDateEvent());
            var venue = ruleBreach.Trades?.Get()?.FirstOrDefault()?.Market?.Name;

            if (oldestPosition == null) oldestPosition = latestPosition;

            if (latestPosition == null) latestPosition = oldestPosition;

            var oldestPositionValue = oldestPosition ?? DateTime.UtcNow;
            var latestPositionValue = latestPosition ?? DateTime.UtcNow;

            if (ruleBreach.UniverseDateTime < oldestPositionValue)
                oldestPositionValue = ruleBreach.UniverseDateTime;

            if (ruleBreach.UniverseDateTime > latestPositionValue)
                latestPositionValue = ruleBreach.UniverseDateTime;

            var trades = ruleBreach.Trades?.Get()?.Select(io => io.ReddeerOrderId).Where(x => x.HasValue)
                .Select(y => y.Value).ToList();

            var ruleBreachObj = new RuleBreach(
                null,
                ruleBreach.RuleParameterId,
                ruleBreach.CorrelationId,
                ruleBreach.IsBackTestRun,
                DateTime.UtcNow,
                ruleBreach.CaseTitle ?? string.Empty,
                ruleBreach.Description ?? string.Empty,
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