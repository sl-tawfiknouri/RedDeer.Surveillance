using System;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Currency.Interfaces
{
    public interface IExchangeRates
    {
        Task<ExchangeRateDto> GetRate(
            Domain.Financial.Currency fixedCurrency,
            Domain.Financial.Currency variableCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}