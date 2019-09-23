namespace Surveillance.Engine.Rules.Currency.Interfaces
{
    using System;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Money;

    using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;

    using Surveillance.Auditing.Context.Interfaces;

    public interface IExchangeRatesService
    {
        Task<ExchangeRateDto> GetRate(
            Currency fixedCurrency,
            Currency variableCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx);
    }
}