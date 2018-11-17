using System;
using System.Collections.Generic;
using Domain.Equity;
using Domain.Finance;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.WashTrade
{
    public class WashTradeRuleBreach : IWashTradeRuleBreach
    {
        public WashTradeRuleBreach(
            IWashTradeRuleParameters parameters,
            ITradePosition tradePosition,
            Security security,
            WashTradeAveragePositionBreach averagePositionBreach,
            WashTradePairingPositionBreach pairingPositionBreach,
            WashTradeClusteringPositionBreach clusteringPositionBreach)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

            Window = parameters.WindowSize;
            Trades = tradePosition;
            Security = security;

            AveragePositionBreach = averagePositionBreach ?? throw new ArgumentNullException(nameof(averagePositionBreach));
            PairingPositionBreach = pairingPositionBreach ?? throw new ArgumentNullException(nameof(pairingPositionBreach));
            ClusteringPositionBreach = clusteringPositionBreach ?? throw new ArgumentNullException(nameof(clusteringPositionBreach));
        }

        public IWashTradeRuleParameters Parameters { get; }

        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public Security Security { get; }

        public WashTradeAveragePositionBreach AveragePositionBreach { get; }
        public WashTradePairingPositionBreach PairingPositionBreach { get; }
        public WashTradeClusteringPositionBreach ClusteringPositionBreach { get; }

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
            public WashTradePairingPositionBreach(bool pairingPositionRuleBreach)
            {
                PairingPositionRuleBreach = pairingPositionRuleBreach;
            }

            public bool PairingPositionRuleBreach { get; }

            public static WashTradePairingPositionBreach None()
            {
                return new WashTradePairingPositionBreach(false);
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