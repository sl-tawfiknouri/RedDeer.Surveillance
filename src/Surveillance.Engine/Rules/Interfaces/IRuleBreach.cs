using System;
using Domain.Core.Financial.Assets;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Interfaces
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
        IFactorValue FactorValue { get; set; }
        IRuleParameter RuleParameters { get; set; }
        DateTime UniverseDateTime { get; set; }
    }
}
