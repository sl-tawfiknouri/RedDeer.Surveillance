using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    public interface IHighProfitsRuleEquitiesParameters : IFilterableRule, IRuleParameter
    {
        TimeSpan WindowSize { get; }
        decimal? HighProfitPercentageThreshold { get; }
        decimal? HighProfitAbsoluteThreshold { get; }
        bool UseCurrencyConversions { get; }
        string HighProfitCurrencyConversionTargetCurrency { get; }
        IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        bool AggregateNonFactorableIntoOwnCategory { get; set; }
    }
}