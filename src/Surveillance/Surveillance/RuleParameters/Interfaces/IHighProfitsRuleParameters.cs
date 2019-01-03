using System;
using System.Collections.Generic;
using Surveillance.RuleParameters.OrganisationalFactors;

namespace Surveillance.RuleParameters.Interfaces
{
    public interface IHighProfitsRuleParameters : IFilterableRule, IRuleParameter
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