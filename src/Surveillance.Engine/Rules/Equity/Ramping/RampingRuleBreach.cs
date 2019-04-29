using System;
using Domain.Core.Financial.Assets;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    public class RampingRuleBreach : IRampingRuleBreach
    {
        public RampingRuleBreach(
            TimeSpan window,
            ITradePosition trades,
            FinancialInstrument security,
            bool isBackTestRun,
            string ruleParameterId,
            string systemOperationId,
            string correlationId,
            IFactorValue factorValue)
        {
            Window = window;
            Trades = trades;
            Security = security;
            IsBackTestRun = isBackTestRun;
            RuleParameterId = ruleParameterId ?? string.Empty;
            SystemOperationId = systemOperationId ?? string.Empty;
            CorrelationId = correlationId ?? string.Empty;
            FactorValue = factorValue;
        }

        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public FinancialInstrument Security { get; }
        public bool IsBackTestRun { get; set; }
        public string RuleParameterId { get; set; }
        public string SystemOperationId { get; set; }
        public string CorrelationId { get; set; }
        public IFactorValue FactorValue { get; set; }
    }
}
