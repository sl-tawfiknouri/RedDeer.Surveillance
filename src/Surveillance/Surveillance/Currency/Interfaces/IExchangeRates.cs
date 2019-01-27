using System;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Currency.Interfaces
{
    public interface IExchangeRates
    {
        Task<ExchangeRateDto> GetRate(
            DomainV2.Financial.Currency fixedCurrency,
            DomainV2.Financial.Currency variableCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}