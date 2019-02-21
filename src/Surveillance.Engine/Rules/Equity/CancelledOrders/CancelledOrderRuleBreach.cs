﻿using System;
using Domain.Financial;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.CancelledOrders
{
    public class CancelledOrderRuleBreach : ICancelledOrderRuleBreach
    {
        public CancelledOrderRuleBreach(
            ISystemProcessOperationContext ctx,
            string correlationId,
            ICancelledOrderRuleEquitiesParameters parameters,
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
            SystemOperationId = ctx.Id.ToString();
            CorrelationId = correlationId;
        }

        public bool HasBreachedRule()
        {
            return ExceededPercentageTradeCountCancellations
                || ExceededPercentagePositionCancellations;
        }

        public ICancelledOrderRuleEquitiesParameters Parameters { get; }
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
        public string SystemOperationId { get; set; }
        public string CorrelationId { get; set; }
    }
}
