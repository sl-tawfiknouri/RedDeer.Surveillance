using System;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Currency.Interfaces
{
    public interface IExchangeRates
    {
        Task<ExchangeRateDto> GetRate(
            Domain.Finance.Currency fixedCurrency,
            Domain.Finance.Currency variableCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}