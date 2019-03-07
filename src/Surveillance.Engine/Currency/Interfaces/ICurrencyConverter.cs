using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Core.Financial.Money;
using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Currency.Interfaces
{
    public interface ICurrencyConverter
    {
        Task<Money?> Convert(
            IReadOnlyCollection<Money> monies,
            Domain.Core.Financial.Money.Currency targetCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}