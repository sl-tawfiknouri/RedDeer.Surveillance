using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.RuleParameters.Tuning;

namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
    [Serializable]
    public class HighVolumeRuleEquitiesParameters : IHighVolumeRuleEquitiesParameters
    {
        public HighVolumeRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? highVolumePercentageDaily,
            decimal? highVolumePercentageWindow,
            decimal? highVolumePercentageMarketCap,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory)
        {
            Id = id ?? string.Empty;

            Windows = new TimeWindows(windowSize);
            HighVolumePercentageDaily = highVolumePercentageDaily;
            HighVolumePercentageWindow = highVolumePercentageWindow;
            HighVolumePercentageMarketCap = highVolumePercentageMarketCap;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
            Funds = RuleFilter.None();
            Strategies = RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
        }

        public HighVolumeRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? highVolumePercentageDaily,
            decimal? highVolumePercentageWindow,
            decimal? highVolumePercentageMarketCap,
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
            HighVolumePercentageDaily = highVolumePercentageDaily;
            HighVolumePercentageWindow = highVolumePercentageWindow;
            HighVolumePercentageMarketCap = highVolumePercentageMarketCap;

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
        [TuneableTimespanParameter]
        public TimeWindows Windows { get; set; }
        [TuneableDecimalParameter]
        public decimal? HighVolumePercentageDaily { get; set; }
        [TuneableDecimalParameter]
        public decimal? HighVolumePercentageWindow { get; set; }
        [TuneableDecimalParameter]
        public decimal? HighVolumePercentageMarketCap { get; set; }
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
                   && (HighVolumePercentageDaily == null
                       || (HighVolumePercentageDaily.GetValueOrDefault() >= 0
                           && HighVolumePercentageDaily.GetValueOrDefault() <= 1))
                   && (HighVolumePercentageWindow == null
                       || (HighVolumePercentageWindow.GetValueOrDefault() >= 0
                           && HighVolumePercentageWindow.GetValueOrDefault() <= 1))
                   && (HighVolumePercentageMarketCap == null
                       || (HighVolumePercentageMarketCap.GetValueOrDefault() >= 0
                           && HighVolumePercentageMarketCap.GetValueOrDefault() <= 1));
        }

        public override int GetHashCode()
        {
            return 
                this.Windows.GetHashCode()
                * this.HighVolumePercentageDaily.GetHashCode()
                * this.HighVolumePercentageWindow.GetHashCode()
                * this.HighVolumePercentageMarketCap.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var castObj = obj as HighVolumeRuleEquitiesParameters;

            if (castObj == null)
            {
                return false;
            }

            return
                this.Windows == castObj.Windows
                && this.HighVolumePercentageDaily == castObj.HighVolumePercentageDaily
                && this.HighVolumePercentageMarketCap == castObj.HighVolumePercentageMarketCap
                && this.HighVolumePercentageWindow == castObj.HighVolumePercentageWindow;
        }
    }
}
