namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome
{
    using System;
    using System.Collections.Generic;

    using Domain.Surveillance.Rules.Tuning;

    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

    /// <summary>
    /// The high volume issuance rule fixed income parameters.
    /// </summary>
    [Serializable]
    public class HighVolumeIssuanceRuleFixedIncomeParameters : IHighVolumeIssuanceRuleFixedIncomeParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighVolumeIssuanceRuleFixedIncomeParameters"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="windowSize">
        /// The window size.
        /// </param>
        /// <param name="accounts">
        /// The accounts.
        /// </param>
        /// <param name="traders">
        /// The traders.
        /// </param>
        /// <param name="markets">
        /// The markets.
        /// </param>
        /// <param name="funds">
        /// The funds.
        /// </param>
        /// <param name="strategies">
        /// The strategies.
        /// </param>
        /// <param name="factors">
        /// The factors.
        /// </param>
        /// <param name="aggregateNonFactorableIntoOwnCategory">
        /// The aggregate non factorable into own category.
        /// </param>
        /// <param name="performTuning">
        /// The perform tuning.
        /// </param>
        public HighVolumeIssuanceRuleFixedIncomeParameters(
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

            this.Factors = factors ?? new List<ClientOrganisationalFactors>();
            this.AggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            this.PerformTuning = performTuning;
        }

        public decimal? FixedIncomeHighVolumePercentageDaily { get; set; }

        public decimal? FixedIncomeHighVolumePercentageWindow { get; set; }

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
        public TunedParameter<string> TunedParameters { get; set; }

        [TuneableTimeWindowParameter]
        public TimeWindows Windows { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var castObj = obj as HighVolumeIssuanceRuleFixedIncomeParameters;

            if (castObj == null) return false;

            return castObj.Windows == this.Windows;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Windows.GetHashCode();
        }

        /// <summary>
        /// The has internal filters check.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasInternalFilters()
        {
            return this.Accounts?.Type != RuleFilterType.None || this.Traders?.Type != RuleFilterType.None
                                                              || this.Markets?.Type != RuleFilterType.None
                                                              || this.Funds?.Type != RuleFilterType.None
                                                              || this.Strategies?.Type != RuleFilterType.None;
        }

        /// <summary>
        /// The valid settings check.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Valid()
        {
            return !string.IsNullOrWhiteSpace(this.Id);
        }
    }
}