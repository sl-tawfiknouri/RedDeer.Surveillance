namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
    using Surveillance.Engine.Rules.Trades.Interfaces;

    public class WashTradeRuleBreach : IWashTradeRuleBreach
    {
        public WashTradeRuleBreach(
            TimeSpan windowSize,
            IFactorValue factorValue,
            ISystemProcessOperationContext operationContext,
            string correlationId,
            IWashTradeRuleParameters equitiesParameters,
            ITradePosition tradePosition,
            FinancialInstrument security,
            WashTradeAveragePositionBreach averagePositionBreach,
            WashTradeClusteringPositionBreach clusteringPositionBreach,
            string description,
            string caseTitle,
            DateTime universeDateTime)
        {
            this.FactorValue = factorValue;
            this.EquitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));

            this.Window = windowSize;
            this.Trades = tradePosition;
            this.Security = security;

            this.AveragePositionBreach =
                averagePositionBreach ?? throw new ArgumentNullException(nameof(averagePositionBreach));
            this.ClusteringPositionBreach = clusteringPositionBreach
                                            ?? throw new ArgumentNullException(nameof(clusteringPositionBreach));

            this.RuleParameterId = equitiesParameters?.Id ?? string.Empty;
            this.SystemOperationId = operationContext.Id.ToString();
            this.CorrelationId = correlationId;
            this.RuleParameters = equitiesParameters;
            this.Description = description ?? string.Empty;
            this.CaseTitle = caseTitle ?? string.Empty;
            this.UniverseDateTime = universeDateTime;
        }

        public WashTradeAveragePositionBreach AveragePositionBreach { get; }

        public string CaseTitle { get; set; }

        public WashTradeClusteringPositionBreach ClusteringPositionBreach { get; }

        public string CorrelationId { get; set; }

        public string Description { get; set; }

        public IWashTradeRuleParameters EquitiesParameters { get; }

        public IFactorValue FactorValue { get; set; }

        public bool IsBackTestRun { get; set; }

        public string RuleParameterId { get; set; }

        public IRuleParameter RuleParameters { get; set; }

        public FinancialInstrument Security { get; }

        public string SystemOperationId { get; set; }

        public ITradePosition Trades { get; }

        public DateTime UniverseDateTime { get; set; }

        public TimeSpan Window { get; }

        public class WashTradeAveragePositionBreach
        {
            public WashTradeAveragePositionBreach(
                bool averagePositionRuleBreach,
                int? averagePositionAmountOfTrades,
                decimal? averagePositionRelativeValueChange,
                Money? averagePositionAbsoluteValueChange)
            {
                this.AveragePositionRuleBreach = averagePositionRuleBreach;
                this.AveragePositionAmountOfTrades = averagePositionAmountOfTrades;
                this.AveragePositionRelativeValueChange = averagePositionRelativeValueChange;
                this.AveragePositionAbsoluteValueChange = averagePositionAbsoluteValueChange;
            }

            public Money? AveragePositionAbsoluteValueChange { get; }

            public int? AveragePositionAmountOfTrades { get; }

            public decimal? AveragePositionRelativeValueChange { get; }

            // Breach by average position
            public bool AveragePositionRuleBreach { get; }

            public static WashTradeAveragePositionBreach None()
            {
                return new WashTradeAveragePositionBreach(false, null, null, null);
            }
        }

        public class WashTradeClusteringPositionBreach
        {
            public WashTradeClusteringPositionBreach(
                bool clusteringPositionBreach,
                int amountOfBreachingClusters,
                IReadOnlyCollection<decimal> centroidsOfBreachingClusters)
            {
                this.ClusteringPositionBreach = clusteringPositionBreach;
                this.AmountOfBreachingClusters = amountOfBreachingClusters;
                this.CentroidsOfBreachingClusters = centroidsOfBreachingClusters ?? new decimal[0];
            }

            public int AmountOfBreachingClusters { get; }

            public IReadOnlyCollection<decimal> CentroidsOfBreachingClusters { get; }

            public bool ClusteringPositionBreach { get; }

            public static WashTradeClusteringPositionBreach None()
            {
                return new WashTradeClusteringPositionBreach(false, 0, null);
            }
        }

        public class WashTradePairingPositionBreach
        {
            public WashTradePairingPositionBreach(
                bool pairingPositionRuleBreach,
                int pairedTradesInBreach,
                int totalTradesPairedUp)
            {
                this.PairingPositionRuleBreach = pairingPositionRuleBreach;
                this.PairedTradesInBreach = pairedTradesInBreach;
                this.PairedTradesTotal = totalTradesPairedUp;
            }

            public int PairedTradesInBreach { get; }

            public int PairedTradesTotal { get; }

            public bool PairingPositionRuleBreach { get; }

            public static WashTradePairingPositionBreach None()
            {
                return new WashTradePairingPositionBreach(false, 0, 0);
            }
        }
    }
}