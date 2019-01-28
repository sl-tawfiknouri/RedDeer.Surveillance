using System;
using System.Collections.Generic;
using DomainV2.Financial;
using DomainV2.Trading;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighProfits
{
    public class HighProfitRuleBreach : IHighProfitRuleBreach
    {
        public HighProfitRuleBreach(
            ISystemProcessOperationContext operationContext,
            string correlationId,
            IHighProfitsRuleParameters parameters,
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
            Window = parameters.WindowSize;
            Parameters = parameters;
            AbsoluteProfits = absoluteProfits;
            AbsoluteProfitCurrency = absoluteProfitCurrency;
            RelativeProfits = relativeProfits;
            Security = security;
            HasAbsoluteProfitBreach = hasAbsoluteProfitBreach;
            HasRelativeProfitBreach = hasRelativeProfitBreach;
            Trades = trades ?? new TradePosition(new List<Order>());
            MarketClosureVirtualProfitComponent = marketClosureVirtualProfitComponent;
            ExchangeRateProfits = profitBreakdown;
            RuleParameterId = parameters?.Id ?? string.Empty;
            SystemOperationId = operationContext.Id.ToString();
            CorrelationId = correlationId;
        }

        public IHighProfitsRuleParameters Parameters { get; }
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
