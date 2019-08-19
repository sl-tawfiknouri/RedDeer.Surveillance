namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    using System;

    using Domain.Core.Financial.Assets;

    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Trades.Interfaces;

    public interface IRuleBreachContext
    {
        string CorrelationId { get; set; }

        IFactorValue FactorValue { get; set; }

        bool IsBackTestRun { get; set; }

        string RuleParameterId { get; set; } // rule parameter primary key on client service

        IRuleParameter RuleParameters { get; set; }

        FinancialInstrument Security { get; }

        string SystemOperationId { get; set; }

        ITradePosition Trades { get; }

        DateTime UniverseDateTime { get; set; }

        TimeSpan Window { get; }
    }
}