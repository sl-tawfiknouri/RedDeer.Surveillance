using System;
using System.Collections.Generic;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade
{
    public class WashTradeRuleBreach : IWashTradeRuleBreach
    {
        public WashTradeRuleBreach(
            IFactorValue factorValue,
            ISystemProcessOperationContext operationContext,
            string correlationId,
            IWashTradeRuleEquitiesParameters equitiesParameters,
            ITradePosition tradePosition,
            FinancialInstrument security,
            WashTradeAveragePositionBreach averagePositionBreach,
            WashTradePairingPositionBreach pairingPositionBreach,
            WashTradeClusteringPositionBreach clusteringPositionBreach)
        {
            FactorValue = factorValue;
            EquitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));


            Window = equitiesParameters.WindowSize;
            Trades = tradePosition;
            Security = security;

            AveragePositionBreach = averagePositionBreach ?? throw new ArgumentNullException(nameof(averagePositionBreach));
            PairingPositionBreach = pairingPositionBreach ?? throw new ArgumentNullException(nameof(pairingPositionBreach));
            ClusteringPositionBreach = clusteringPositionBreach ?? throw new ArgumentNullException(nameof(clusteringPositionBreach));

            RuleParameterId = equitiesParameters?.Id ?? string.Empty;
            SystemOperationId = operationContext.Id.ToString();
            CorrelationId = correlationId;
        }

        public IWashTradeRuleEquitiesParameters EquitiesParameters { get; }

        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public FinancialInstrument Security { get; }

        public WashTradeAveragePositionBreach AveragePositionBreach { get; }
        public WashTradePairingPositionBreach PairingPositionBreach { get; }
        public WashTradeClusteringPositionBreach ClusteringPositionBreach { get; }

        public bool IsBackTestRun { get; set; }
        public string RuleParameterId { get; set; }
        public string SystemOperationId { get; set; }
        public string CorrelationId { get; set; }
        public IFactorValue FactorValue { get; set; }


        public class WashTradeAveragePositionBreach
        {
            public WashTradeAveragePositionBreach(
                bool averagePositionRuleBreach,
                int? averagePositionAmountOfTrades,
                decimal? averagePositionRelativeValueChange,
                CurrencyAmount? averagePositionAbsoluteValueChange)
            {
                AveragePositionRuleBreach = averagePositionRuleBreach;
                AveragePositionAmountOfTrades = averagePositionAmountOfTrades;
                AveragePositionRelativeValueChange = averagePositionRelativeValueChange;
                AveragePositionAbsoluteValueChange = averagePositionAbsoluteValueChange;
            }

            // Breach by average position
            public bool AveragePositionRuleBreach { get; }
            public int? AveragePositionAmountOfTrades { get; }
            public decimal? AveragePositionRelativeValueChange { get; }
            public CurrencyAmount? AveragePositionAbsoluteValueChange { get; }

            public static WashTradeAveragePositionBreach None()
            {
                return new WashTradeAveragePositionBreach(false, null, null, null);
            }
        }

        public class WashTradePairingPositionBreach
        {
            public WashTradePairingPositionBreach(bool pairingPositionRuleBreach, int pairedTradesInBreach, int totalTradesPairedUp)
            {
                PairingPositionRuleBreach = pairingPositionRuleBreach;
                PairedTradesInBreach = pairedTradesInBreach;
                PairedTradesTotal = totalTradesPairedUp;
            }

            public bool PairingPositionRuleBreach { get; }
            public int PairedTradesInBreach { get; }
            public int PairedTradesTotal { get; }

            public static WashTradePairingPositionBreach None()
            {
                return new WashTradePairingPositionBreach(false, 0, 0);
            }
        }

        public class WashTradeClusteringPositionBreach
        {
            public WashTradeClusteringPositionBreach(
                bool clusteringPositionBreach,
                int amountOfBreachingClusters,
                IReadOnlyCollection<decimal> centroidsOfBreachingClusters)
            {
                ClusteringPositionBreach = clusteringPositionBreach;
                AmountOfBreachingClusters = amountOfBreachingClusters;
                CentroidsOfBreachingClusters = centroidsOfBreachingClusters ?? new decimal[0];
            }

            public bool ClusteringPositionBreach { get; }
            public int AmountOfBreachingClusters { get; }
            public IReadOnlyCollection<decimal> CentroidsOfBreachingClusters { get; }

            public static WashTradeClusteringPositionBreach None()
            {
                return new WashTradeClusteringPositionBreach(false, 0, null);
            }
        }
    }
}