using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface ISpoofingRuleEquitiesParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable, IReferenceDataFilterable, IMarketCapFilterable
    {
        decimal CancellationThreshold { get; }
        decimal RelativeSizeMultipleForSpoofExceedingReal { get; }
        TimeWindows Windows { get; }
    }
}