using System;
using System.Collections.Generic;
using Domain.Financial;
using Domain.Trading;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    public class HighProfitRuleBreach : IHighProfitRuleBreach
    {
        public HighProfitRuleBreach(
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
            Window = equitiesParameters.WindowSize;
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
    }
}
