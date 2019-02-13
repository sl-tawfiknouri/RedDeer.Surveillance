using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Financial;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Currency.Interfaces
{
    public interface ICurrencyConverter
    {
        Task<CurrencyAmount?> Convert(IReadOnlyCollection<CurrencyAmount> currencyAmounts, DomainV2.Financial.Currency targetCurrency, DateTime dayOfConversion, ISystemProcessOperationRunRuleContext ruleCtx);
    }
}