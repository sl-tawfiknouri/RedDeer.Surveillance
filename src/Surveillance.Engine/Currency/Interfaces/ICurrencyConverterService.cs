namespace Surveillance.Engine.Rules.Currency.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Money;

    using Surveillance.Auditing.Context.Interfaces;

    public interface ICurrencyConverterService
    {
        Task<Money?> Convert(
            IReadOnlyCollection<Money> monies,
            Currency targetCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}