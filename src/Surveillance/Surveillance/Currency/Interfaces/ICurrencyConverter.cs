using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Finance;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Currency.Interfaces
{
    public interface ICurrencyConverter
    {
        Task<CurrencyAmount?> Convert(IReadOnlyCollection<CurrencyAmount> currencyAmounts, Domain.Finance.Currency targetCurrency, DateTime dayOfConversion, ISystemProcessOperationRunRuleContext ruleCtx);
    }
}