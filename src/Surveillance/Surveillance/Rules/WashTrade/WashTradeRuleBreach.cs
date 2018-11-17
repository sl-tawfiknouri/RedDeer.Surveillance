using System;
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
            WashTradePairingPositionBreach pairingPositionBreach)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

            Window = parameters.WindowSize;
            Trades = tradePosition;
            Security = security;

            AveragePositionBreach = averagePositionBreach ?? throw new ArgumentNullException(nameof(averagePositionBreach));
            PairingPositionBreach = pairingPositionBreach ?? throw new ArgumentNullException(nameof(pairingPositionBreach));
        }

        public IWashTradeRuleParameters Parameters { get; }

        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public Security Security { get; }

        public WashTradeAveragePositionBreach AveragePositionBreach { get; }
        public WashTradePairingPositionBreach PairingPositionBreach { get; }

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
            public WashTradePairingPositionBreach(
                bool pairingPositionRuleBreach)
            {
                PairingPositionRuleBreach = pairingPositionRuleBreach;
            }

            public bool PairingPositionRuleBreach { get; }

            public static WashTradePairingPositionBreach None()
            {
                return new WashTradePairingPositionBreach(false);
            }
        }
    }
}
