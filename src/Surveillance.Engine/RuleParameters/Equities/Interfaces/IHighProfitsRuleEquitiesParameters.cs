﻿using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface IHighProfitsRuleEquitiesParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable, IReferenceDataFilterable
    {
        TimeSpan WindowSize { get; }
        bool PerformHighProfitWindowAnalysis { get; }
        bool PerformHighProfitDailyAnalysis { get; }
        decimal? HighProfitPercentageThreshold { get; }
        decimal? HighProfitAbsoluteThreshold { get; }
        bool UseCurrencyConversions { get; }
        string HighProfitCurrencyConversionTargetCurrency { get; }
    }
}