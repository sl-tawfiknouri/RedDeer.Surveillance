using System;
using Domain.Equity;
using Surveillance.Rules.Cancelled_Orders.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Cancelled_Orders
{
    public class CancelledOrderRuleBreach : ICancelledOrderRuleBreach
    {
        public CancelledOrderRuleBreach(
            ICancelledOrderRuleParameters parameters,
            ITradePosition trades,
            Security security,
            bool exceededPercentagePositionCancellations,
            decimal? percentagePositionCancelled,
            bool exceededPercentageTradeCountCancellations,
            decimal? percentageTradeCountCancelled)
        {
            Parameters = parameters;
            Trades = trades;
            Security = security;
            ExceededPercentagePositionCancellations = exceededPercentagePositionCancellations;
            PercentagePositionCancelled = percentagePositionCancelled;
            ExceededPercentageTradeCountCancellations = exceededPercentageTradeCountCancellations;
            PercentageTradeCountCancelled = percentageTradeCountCancelled;
            Window = parameters.WindowSize;
        }

        public bool HasBreachedRule()
        {
            return ExceededPercentageTradeCountCancellations
                || ExceededPercentagePositionCancellations;
        }

        public ICancelledOrderRuleParameters Parameters { get; }
        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public Security Security { get; }


        public bool ExceededPercentagePositionCancellations { get; }
        public decimal? PercentagePositionCancelled { get; }
        public bool ExceededPercentageTradeCountCancellations { get; }
        public decimal? PercentageTradeCountCancelled { get; }
    }
}
