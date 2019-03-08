using System;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Currency.Interfaces
{
    public interface IExchangeRatesService
    {
        Task<ExchangeRateDto> GetRate(
            Domain.Core.Financial.Money.Currency fixedCurrency,
            Domain.Core.Financial.Money.Currency variableCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}