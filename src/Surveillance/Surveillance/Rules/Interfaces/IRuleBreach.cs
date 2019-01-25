using System;
using DomainV2.Financial;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Interfaces
{
    public interface IRuleBreach
    {
        TimeSpan Window { get; }
        ITradePosition Trades { get; }
        FinancialInstrument Security { get; }
        bool IsBackTestRun { get; set; }
        string RuleParameterId { get; set; } // rule parameter primary key on client service
        string SystemOperationId { get; set; }
        string CorrelationId { get; set; }
    }
}
