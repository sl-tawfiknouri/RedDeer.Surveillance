﻿using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.RuleParameters.Tuning;

namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
    [Serializable]
    public class SpoofingRuleEquitiesParameters : ISpoofingRuleEquitiesParameters
    {
        public SpoofingRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal cancellationThreshold,
            decimal relativeSizeMultipleForSpoofingExceedingReal,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            Windows = new TimeWindows(windowSize);
            CancellationThreshold = cancellationThreshold;
            RelativeSizeMultipleForSpoofExceedingReal = relativeSizeMultipleForSpoofingExceedingReal;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
            Funds = RuleFilter.None();
            Strategies = RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public SpoofingRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal cancellationThreshold,
            decimal relativeSizeMultipleForSpoofingExceedingReal,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategies,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            Windows = new TimeWindows(windowSize);
            CancellationThreshold = cancellationThreshold;
            RelativeSizeMultipleForSpoofExceedingReal = relativeSizeMultipleForSpoofingExceedingReal;

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
            Funds = funds ?? RuleFilter.None();
            Strategies = strategies ?? RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        [TuneableIdParameter]
        public string Id { get; set; }
        [TuneableTimeWindowParameter]
        public TimeWindows Windows { get; set; }
        [TuneableDecimalParameter]
        public decimal CancellationThreshold { get; set; }
        [TuneableDecimalParameter]
        public decimal RelativeSizeMultipleForSpoofExceedingReal { get; set; }

        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }
        public RuleFilter Funds { get; set; }
        public RuleFilter Strategies { get; set; }

        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public bool HasFilters()
        {
            return
                Accounts?.Type != RuleFilterType.None
                || Traders?.Type != RuleFilterType.None
                || Markets?.Type != RuleFilterType.None
                || Funds?.Type != RuleFilterType.None
                || Strategies?.Type != RuleFilterType.None;
        }

        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(Id)
               && CancellationThreshold >= 0
               && CancellationThreshold <= 1
               && RelativeSizeMultipleForSpoofExceedingReal >= 0;
        }

        public override int GetHashCode()
        {
            return Windows.GetHashCode()
               * CancellationThreshold.GetHashCode()
               * RelativeSizeMultipleForSpoofExceedingReal.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var castObj = obj as SpoofingRuleEquitiesParameters;

            if (castObj == null)
            {
                return false;
            }

            return Windows == castObj.Windows
                   && CancellationThreshold == castObj.CancellationThreshold
                   && RelativeSizeMultipleForSpoofExceedingReal == castObj.RelativeSizeMultipleForSpoofExceedingReal;

        }

        public bool IsTuned { get; set; }
        [TunedParam]
        public TunedParameter<string> TunedParam { get; set; }
    }
}
