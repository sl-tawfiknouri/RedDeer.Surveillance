using System;
using DomainV2.Financial;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.CancelledOrders.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.CancelledOrders
{
    public class CancelledOrderRuleBreach : ICancelledOrderRuleBreach
    {
        public CancelledOrderRuleBreach(
            ICancelledOrderRuleParameters parameters,
            ITradePosition trades,
            FinancialInstrument security,
            bool exceededPercentagePositionCancellations,
            decimal? percentagePositionCancelled,
            int? amountOfPositionCancelled,
            int? amountOfPositionInTotal,
            bool exceededPercentageTradeCountCancellations,
            decimal? percentageTradeCountCancelled)
        {
            Parameters = parameters;
            Trades = trades;
            Security = security;
            ExceededPercentagePositionCancellations = exceededPercentagePositionCancellations;
            PercentagePositionCancelled = percentagePositionCancelled;
            AmountOfPositionCancelled = amountOfPositionCancelled;
            AmountOfPositionInTotal = amountOfPositionInTotal;
            ExceededPercentageTradeCountCancellations = exceededPercentageTradeCountCancellations;
            PercentageTradeCountCancelled = percentageTradeCountCancelled;
            Window = parameters.WindowSize;
            RuleParameterId = Parameters?.Id ?? string.Empty;
        }

        public bool HasBreachedRule()
        {
            return ExceededPercentageTradeCountCancellations
                || ExceededPercentagePositionCancellations;
        }

        public ICancelledOrderRuleParameters Parameters { get; }
        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public FinancialInstrument Security { get; }
        public bool ExceededPercentagePositionCancellations { get; }
        public decimal? PercentagePositionCancelled { get; }
        public int? AmountOfPositionCancelled { get; }
        public int? AmountOfPositionInTotal { get; }
        public bool ExceededPercentageTradeCountCancellations { get; }
        public decimal? PercentageTradeCountCancelled { get; }

        public bool IsBackTestRun { get; set; }
        public string RuleParameterId { get; set; }
    }
}
