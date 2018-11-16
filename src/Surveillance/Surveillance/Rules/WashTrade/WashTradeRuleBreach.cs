using System;
using Domain.Finance;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades;

namespace Surveillance.Rules.WashTrade
{
    public class WashTradeRuleBreach : IWashTradeRuleBreach
    {
        public WashTradeRuleBreach(
            IWashTradeRuleParameters parameters,
            TradePosition breachingTradePosition,
            bool averagePositionRuleBreach,
            int? averagePositionAmountOfTrades,
            decimal? averagePositionRelativeValueChange,
            CurrencyAmount? averagePositionAbsoluteValueChange)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

            BreachingTradePosition = breachingTradePosition;
            AveragePositionRuleBreach = averagePositionRuleBreach;
            AveragePositionAmountOfTrades = averagePositionAmountOfTrades;
            AveragePositionRelativeValueChange = averagePositionRelativeValueChange;
            AveragePositionAbsoluteValueChange = averagePositionAbsoluteValueChange;
        }

        public IWashTradeRuleParameters Parameters { get; }

        public TradePosition BreachingTradePosition { get; }

        // Breach by average position
        public bool AveragePositionRuleBreach { get; }
        public int? AveragePositionAmountOfTrades { get; }
        public decimal? AveragePositionRelativeValueChange { get; }
        public CurrencyAmount? AveragePositionAbsoluteValueChange { get; }
    }
}
