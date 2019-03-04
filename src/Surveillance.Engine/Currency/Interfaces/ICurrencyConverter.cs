using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Currency.Interfaces
{
    public interface ICurrencyConverter
    {
        Task<Money?> Convert(IReadOnlyCollection<Money> Moneys, Domain.Financial.Currency targetCurrency, DateTime dayOfConversion, ISystemProcessOperationRunRuleContext ruleCtx);
    }
}