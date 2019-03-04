using System;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Currency.Interfaces
{
    public interface IExchangeRates
    {
        Task<ExchangeRateDto> GetRate(
            Domain.Core.Financial.Currency fixedCurrency,
            Domain.Core.Financial.Currency variableCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}