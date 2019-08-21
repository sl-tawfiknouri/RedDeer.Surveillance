namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome
{
    using System;
    using System.Collections.Generic;

    using Domain.Surveillance.Rules.Tuning;

    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

    [Serializable]
    public class HighProfitsRuleFixedIncomeParameters : IHighProfitsRuleFixedIncomeParameters
    {
        public HighProfitsRuleFixedIncomeParameters(
            string id,
            TimeSpan windowSize,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategies,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            bool performTuning)
        {
            this.Id = id ?? string.Empty;
            this.Windows = new TimeWindows(id, windowSize);
            this.Accounts = accounts ?? RuleFilter.None();
            this.Traders = traders ?? RuleFilter.None();
            this.Markets = markets ?? RuleFilter.None();
            this.Funds = funds ?? RuleFilter.None();
            this.Strategies = strategies ?? RuleFilter.None();
            this.Factors = factors ?? new ClientOrganisationalFactors[0];
            this.AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
            this.PerformTuning = performTuning;
        }

        public RuleFilter Accounts { get; set; }

        public bool AggregateNonFactorableIntoOwnCategory { get; set; }

        public IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }

        public RuleFilter Funds { get; set; }

        [TuneableIdParameter]
        public string Id { get; set; }

        public RuleFilter Markets { get; set; }

        public bool PerformTuning { get; set; }

        public RuleFilter Strategies { get; set; }

        public RuleFilter Traders { get; set; }

        [TunedParam]
        public TunedParameter<string> TunedParam { get; set; }

        [TuneableTimeWindowParameter]
        public TimeWindows Windows { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var castObj = obj as HighProfitsRuleFixedIncomeParameters;

            if (castObj == null) return false;

            return this.Windows == castObj.Windows;
        }

        public override int GetHashCode()
        {
            return this.Windows.GetHashCode();
        }

        public bool HasInternalFilters()
        {
            return this.Accounts?.Type != RuleFilterType.None || this.Traders?.Type != RuleFilterType.None
                                                              || this.Markets?.Type != RuleFilterType.None
                                                              || this.Funds?.Type != RuleFilterType.None
                                                              || this.Strategies?.Type != RuleFilterType.None;
        }

        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(this.Id);
        }
    }
}