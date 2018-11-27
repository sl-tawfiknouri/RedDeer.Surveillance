using System;

namespace Surveillance.RuleParameters.Interfaces
{
    public interface IHighProfitsRuleParameters : IFilterableRule
    {
        TimeSpan WindowSize { get; }
        decimal? HighProfitPercentageThreshold { get; }
        decimal? HighProfitAbsoluteThreshold { get; }
        bool UseCurrencyConversions { get; }
        string HighProfitCurrencyConversionTargetCurrency { get; }
    }
}