﻿using System;
using System.Collections.Generic;
using Domain.Core.Financial.Assets;
using Domain.Core.Trading.Orders;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    public class HighProfitRuleBreach : IHighProfitRuleBreach
    {
        public HighProfitRuleBreach(
            IFactorValue factorValue,
            ISystemProcessOperationContext operationContext,
            string correlationId,
            IHighProfitsRuleEquitiesParameters equitiesParameters,
            decimal? absoluteProfits,
            string absoluteProfitCurrency,
            decimal? relativeProfits,
            FinancialInstrument security,
            bool hasAbsoluteProfitBreach,
            bool hasRelativeProfitBreach,
            ITradePosition trades,
            bool marketClosureVirtualProfitComponent,
            IExchangeRateProfitBreakdown profitBreakdown)
        {
            FactorValue = factorValue;
            Window = equitiesParameters.Windows.BackwardWindowSize;
            EquitiesParameters = equitiesParameters;
            AbsoluteProfits = absoluteProfits;
            AbsoluteProfitCurrency = absoluteProfitCurrency;
            RelativeProfits = relativeProfits;
            Security = security;
            HasAbsoluteProfitBreach = hasAbsoluteProfitBreach;
            HasRelativeProfitBreach = hasRelativeProfitBreach;
            Trades = trades ?? new TradePosition(new List<Order>());
            MarketClosureVirtualProfitComponent = marketClosureVirtualProfitComponent;
            ExchangeRateProfits = profitBreakdown;
            RuleParameterId = equitiesParameters?.Id ?? string.Empty;
            SystemOperationId = operationContext.Id.ToString();
            CorrelationId = correlationId;
            RuleParameters = equitiesParameters;
        }

        public IHighProfitsRuleEquitiesParameters EquitiesParameters { get; }
        public bool HasAbsoluteProfitBreach { get; }
        public bool HasRelativeProfitBreach { get; }
        public decimal? AbsoluteProfits { get; }
        public string AbsoluteProfitCurrency { get; }
        public decimal? RelativeProfits { get; }
        public bool MarketClosureVirtualProfitComponent { get; }
        public FinancialInstrument Security { get; }
        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public IExchangeRateProfitBreakdown ExchangeRateProfits { get; }

        public bool IsBackTestRun { get; set; }
        public string RuleParameterId { get; set; }
        public string SystemOperationId { get; set; }
        public string CorrelationId { get; set; }
        public IFactorValue FactorValue { get; set; }
        public IRuleParameter RuleParameters { get; set; }
    }
}
