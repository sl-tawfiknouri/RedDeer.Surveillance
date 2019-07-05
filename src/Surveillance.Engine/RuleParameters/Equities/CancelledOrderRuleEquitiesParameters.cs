using System;
using System.Collections.Generic;
using Domain.Surveillance.Rules.Tuning;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Extensions;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.RuleParameters.Tuning;

namespace Surveillance.Engine.Rules.RuleParameters.Equities
{
    [Serializable]
    public class CancelledOrderRuleEquitiesParameters : ICancelledOrderRuleEquitiesParameters
    {
        public CancelledOrderRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? cancelledOrderPositionPercentageThreshold,
            decimal? cancelledOrderCountPercentageThreshold,
            int minimumNumberOfTradesToApplyRuleTo,
            int? maximumNumberOfTradesToApplyRuleTo,
            DecimalRangeRuleFilter marketCapFilter,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategy,
            RuleFilter sectors,
            RuleFilter industries,
            RuleFilter regions,
            RuleFilter countries,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            bool performTuning)
        {
            Id = id ?? string.Empty;

            Windows = new TimeWindows(id, windowSize);
            CancelledOrderPercentagePositionThreshold = cancelledOrderPositionPercentageThreshold;
            CancelledOrderCountPercentageThreshold = cancelledOrderCountPercentageThreshold;
            MinimumNumberOfTradesToApplyRuleTo = minimumNumberOfTradesToApplyRuleTo;
            MaximumNumberOfTradesToApplyRuleTo = maximumNumberOfTradesToApplyRuleTo;

            MarketCapFilter = marketCapFilter ?? DecimalRangeRuleFilter.None();

            Accounts = accounts ?? RuleFilter.None();
            Traders = traders ?? RuleFilter.None();
            Markets = markets ?? RuleFilter.None();
            Funds = funds ?? RuleFilter.None();
            Strategies = strategy ?? RuleFilter.None();

            Sectors = sectors ?? RuleFilter.None();
            Industries = industries ?? RuleFilter.None();
            Regions = regions ?? RuleFilter.None();
            Countries = countries ?? RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            PerformTuning = performTuning;
        }

        public CancelledOrderRuleEquitiesParameters(
            string id,
            TimeSpan windowSize,
            decimal? cancelledOrderPositionPercentageThreshold,
            decimal? cancelledOrderCountPercentageThreshold,
            int minimumNumberOfTradesToApplyRuleTo,
            int? maximumNumberOfTradesToApplyRuleTo,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            bool performTuning)
        {
            Id = id ?? string.Empty;
            Windows = new TimeWindows(id, windowSize);
            CancelledOrderPercentagePositionThreshold = cancelledOrderPositionPercentageThreshold;
            CancelledOrderCountPercentageThreshold = cancelledOrderCountPercentageThreshold;
            MinimumNumberOfTradesToApplyRuleTo = minimumNumberOfTradesToApplyRuleTo;
            MaximumNumberOfTradesToApplyRuleTo = maximumNumberOfTradesToApplyRuleTo;

            MarketCapFilter = DecimalRangeRuleFilter.None();

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
            Funds = RuleFilter.None();
            Strategies = RuleFilter.None();

            Sectors = RuleFilter.None();
            Industries = RuleFilter.None();
            Regions = RuleFilter.None();
            Countries = RuleFilter.None();

            Factors = factors ?? new ClientOrganisationalFactors[0];
            AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            PerformTuning = performTuning;
        }

        [TuneableIdParameter]
        public string Id { get; set; }
        [TuneableTimeWindowParameter]
        public TimeWindows Windows { get; set; }
        [TuneableDecimalParameter]
        public decimal? CancelledOrderPercentagePositionThreshold { get; set; }
        [TuneableDecimalParameter]
        public decimal? CancelledOrderCountPercentageThreshold { get; set; }
        [TuneableIntegerParameter]
        public int MinimumNumberOfTradesToApplyRuleTo { get; set; }
        [TuneableIntegerParameter]
        public int? MaximumNumberOfTradesToApplyRuleTo { get; set; }
        public DecimalRangeRuleFilter MarketCapFilter { get; }
        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }
        public RuleFilter Funds { get; set; }
        public RuleFilter Strategies { get; set; }

        public RuleFilter Sectors { get; set; }
        public RuleFilter Industries { get; set; }
        public RuleFilter Regions { get; set; }
        public RuleFilter Countries { get; set; }

        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public bool HasInternalFilters()
            => IFilterableRuleExtensions.HasInternalFilters(this);

        public bool HasMarketCapFilters()
            => IMarketCapFilterableExtensions.HasMarketCapFilters(this);

        public bool HasReferenceDataFilters()
            => IReferenceDataFilterableExtensions.HasReferenceDataFilters(this);
        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(Id)
                   && (CancelledOrderPercentagePositionThreshold == null
                       || (CancelledOrderPercentagePositionThreshold.Value <= 1
                           && CancelledOrderPercentagePositionThreshold.Value >= 0))
                   && (CancelledOrderCountPercentageThreshold == null
                       || (CancelledOrderCountPercentageThreshold.Value >= 0
                           && CancelledOrderCountPercentageThreshold <= 1))
                && MinimumNumberOfTradesToApplyRuleTo >= 2
                && (MaximumNumberOfTradesToApplyRuleTo == null
                    || MaximumNumberOfTradesToApplyRuleTo.GetValueOrDefault() >= MinimumNumberOfTradesToApplyRuleTo);
        }

        public override int GetHashCode()
        {
            return Windows.GetHashCode()
                   * CancelledOrderPercentagePositionThreshold.GetHashCode()
                   * CancelledOrderCountPercentageThreshold.GetHashCode()
                   * MinimumNumberOfTradesToApplyRuleTo.GetHashCode()
                   * MaximumNumberOfTradesToApplyRuleTo.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var castObj = obj as CancelledOrderRuleEquitiesParameters;

            if (castObj == null)
            {
                return false;
            }

            return
                this.Windows == castObj.Windows
                && this.CancelledOrderPercentagePositionThreshold == castObj.CancelledOrderPercentagePositionThreshold
                && this.CancelledOrderCountPercentageThreshold == castObj.CancelledOrderCountPercentageThreshold
                && this.MinimumNumberOfTradesToApplyRuleTo == castObj.MinimumNumberOfTradesToApplyRuleTo
                && this.MaximumNumberOfTradesToApplyRuleTo == castObj.MaximumNumberOfTradesToApplyRuleTo;
        }

        public bool PerformTuning { get; set; }

        [TunedParam]
        public TunedParameter<string> TunedParam { get; set; }
    }
}