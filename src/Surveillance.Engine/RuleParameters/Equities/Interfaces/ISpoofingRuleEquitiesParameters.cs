using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface ISpoofingRuleEquitiesParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable
    {
        decimal CancellationThreshold { get; }
        decimal RelativeSizeMultipleForSpoofExceedingReal { get; }
        TimeSpan WindowSize { get; }
    }
}