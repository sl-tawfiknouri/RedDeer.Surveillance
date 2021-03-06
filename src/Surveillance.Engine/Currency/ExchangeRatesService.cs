﻿namespace Surveillance.Engine.Rules.Currency
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Money;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;

    /// <summary>
    ///     Will fetch the correct x-rate for a pair of currencies on a given date
    ///     The quotation EUR/USD 1.3225 means that 1 Euro will buy 1.3225 US dollars.
    ///     In other words, this is the price of a unit of Euro in US dollars.
    ///     Here, EUR is called the "Fixed currency", while USD is called the "Variable currency".
    /// </summary>
    public class ExchangeRatesService : IExchangeRatesService
    {
        private readonly IExchangeRateApiCachingDecorator _exchangeRateApiRepository;

        private readonly ILogger<ExchangeRatesService> _logger;

        public ExchangeRatesService(
            IExchangeRateApiCachingDecorator exchangeRateApiRepository,
            ILogger<ExchangeRatesService> logger)
        {
            this._exchangeRateApiRepository = exchangeRateApiRepository
                                              ?? throw new ArgumentNullException(nameof(exchangeRateApiRepository));

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // fixed is basically from
        // variable is to
        // so EUR/USD 1.3225 means 1 euro buys 1.3225 dollars
        // with eur = fixed and usd = variable currencies
        public async Task<ExchangeRateDto> GetRate(
            Currency fixedCurrency,
            Currency variableCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            if (string.IsNullOrWhiteSpace(fixedCurrency.Code) || string.IsNullOrWhiteSpace(variableCurrency.Code))
            {
                this._logger.LogError(
                    $"was asked to convert two currencies. Once of which was null or empty {fixedCurrency} {variableCurrency}");
                return null;
            }

            if (string.Equals(fixedCurrency.Code, variableCurrency.Code, StringComparison.InvariantCultureIgnoreCase))
            {
                var noConversionRate = new ExchangeRateDto
                                           {
                                               DateTime = dayOfConversion,
                                               FixedCurrency = fixedCurrency.Code,
                                               VariableCurrency = variableCurrency.Code,
                                               Rate = 1
                                           };

                this._logger.LogInformation(
                    $"was asked to convert two currencies but they were equal. Returning a rate of 1 for {fixedCurrency} and {variableCurrency}");

                return noConversionRate;
            }

            var rates = await this.GetExchangeRatesNearestToDate(dayOfConversion, ruleCtx);

            if (rates == null || !rates.Any())
            {
                this._logger.LogError($"unable to find any rates on {dayOfConversion.ToShortDateString()}");
                ruleCtx.EventException(
                    $"unable to change rates from {fixedCurrency.Code} to {variableCurrency.Code} on {dayOfConversion.ToShortDateString()}");

                return null;
            }

            var rate = this.Convert(rates, fixedCurrency, variableCurrency, dayOfConversion, ruleCtx);

            this._logger.LogInformation(
                $"was asked to convert two currencies {fixedCurrency} and {variableCurrency} on {dayOfConversion}. Returning {rate.Rate} as the exchange rate");

            return rate;
        }

        private ExchangeRateDto Convert(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Currency fixedCurrency,
            Currency variableCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            // direct exchange rate i.e. we want to do USD to GBP and we have USD / GBP
            var directConversion = this.TryDirectConversion(exchangeRates, fixedCurrency, variableCurrency);

            if (directConversion != null)
            {
                this._logger.LogInformation(
                    $"was able to directly convert {fixedCurrency} to {variableCurrency} at rate of {directConversion.Rate} on {directConversion.DateTime}");

                return directConversion;
            }

            // reciprocal exchange rate i.e. we want to do USD to GBP but we have GBP / USD
            var reciprocalConversion = this.TryReciprocalConversion(exchangeRates, fixedCurrency, variableCurrency);

            if (reciprocalConversion != null)
            {
                this._logger.LogInformation(
                    $"was able to reciprocally convert {fixedCurrency} to {variableCurrency} at rate of {reciprocalConversion.Rate} on {reciprocalConversion.DateTime}");

                return reciprocalConversion;
            }

            // implicit exchange rate i.e. we want to do EUR to GBP but we have EUR / USD and GBP / USD
            var indirectConversion = this.TryIndirectConversion(
                exchangeRates,
                fixedCurrency,
                variableCurrency,
                dayOfConversion,
                ruleCtx);

            if (indirectConversion == null)
            {
                this._logger.LogError(
                    $"was unable to convert {fixedCurrency.Code} to {variableCurrency.Code} on {dayOfConversion}");
                ruleCtx.EventException(
                    $"was unable to convert {fixedCurrency.Code} to {variableCurrency.Code} on {dayOfConversion}");

                return null;
            }

            this._logger.LogInformation(
                $"was able to indirectly convert {fixedCurrency} to {variableCurrency} at rate of {indirectConversion.Rate} on {indirectConversion.DateTime}");

            return indirectConversion;
        }

        private List<ExchangeRateDto> GetExchangeRates(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Currency targetCurrency)
        {
            return exchangeRates.Where(
                    er => string.Equals(
                        er.FixedCurrency,
                        targetCurrency.Code,
                        StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<IReadOnlyCollection<ExchangeRateDto>> GetExchangeRatesNearestToDate(
            DateTime dayOfRate,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            dayOfRate = dayOfRate.Date;
            var exchRate = await this._exchangeRateApiRepository.GetAsync(dayOfRate, dayOfRate);

            // cycle through last two weeks of exchange rates
            var offset = 0;
            var cycleDate = dayOfRate;
            while (!exchRate.ContainsKey(cycleDate) && offset < 15)
            {
                offset += 1;
                cycleDate = cycleDate.AddDays(-1);
            }

            if (offset > 14)
            {
                this._logger.LogError($"could not find an exchange rate in the date range around {dayOfRate}.");
                ruleCtx.EventException($"could not find an exchange rate in the date range around {dayOfRate}.");

                return new ExchangeRateDto[0];
            }

            if (!exchRate.TryGetValue(cycleDate, out var rates))
            {
                this._logger.LogError(
                    $"could not find an exchange rate in the date range around {dayOfRate} in the dictionary.");
                ruleCtx.EventException(
                    $"could not find an exchange rate in the date range around {dayOfRate} in the dictionary.");

                return new ExchangeRateDto[0];
            }

            return rates;
        }

        private ExchangeRateDto TryDirectConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Currency fixedCurrency,
            Currency variableCurrency)
        {
            var directConversion = exchangeRates.FirstOrDefault(
                er => string.Equals(er.FixedCurrency, fixedCurrency.Code, StringComparison.InvariantCultureIgnoreCase)
                      && string.Equals(
                          er.VariableCurrency,
                          variableCurrency.Code,
                          StringComparison.InvariantCultureIgnoreCase));

            return directConversion;
        }

        private ExchangeRateDto TryIndirectConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Currency fixedCurrency,
            Currency variableCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            var fixedExchangeRates = this.GetExchangeRates(exchangeRates, fixedCurrency);
            var variableExchangeRates = this.GetExchangeRates(exchangeRates, variableCurrency);

            var sharedVariableRateInitial = fixedExchangeRates.FirstOrDefault(
                ier => variableExchangeRates.Select(ter => ter.VariableCurrency).Contains(ier.VariableCurrency));

            if (sharedVariableRateInitial == null)
            {
                this._logger.LogError(
                    $"could not find a shared common currency using a one step approach for {fixedCurrency.Code} and {variableCurrency.Code} on {dayOfConversion}");

                ruleCtx.EventException(
                    $"could not find a shared common currency using a one step approach for {fixedCurrency.Code} and {variableCurrency.Code} on {dayOfConversion}");

                return null;
            }

            var sharedVariableRateTarget = variableExchangeRates.FirstOrDefault(
                tec => string.Equals(
                    tec.VariableCurrency,
                    sharedVariableRateInitial.VariableCurrency,
                    StringComparison.InvariantCultureIgnoreCase));

            if (sharedVariableRateTarget == null)
            {
                this._logger.LogError(
                    $"could not find a shared common currency using a one step approach for {fixedCurrency.Code} and {variableCurrency.Code} on {dayOfConversion}");

                ruleCtx.EventException(
                    $"could not find a shared common currency using a one step approach for {fixedCurrency.Code} and {variableCurrency.Code} on {dayOfConversion}");

                return null;
            }

            var reciprocalExchangeRate =

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                sharedVariableRateTarget.Rate != 0 ? 1 / (decimal)sharedVariableRateTarget.Rate : 0;

            var completeRate = (double)reciprocalExchangeRate * sharedVariableRateInitial.Rate;

            return new ExchangeRateDto
                       {
                           DateTime = sharedVariableRateTarget.DateTime,
                           FixedCurrency = fixedCurrency.Code,
                           VariableCurrency = variableCurrency.Code,
                           Rate = completeRate
                       };
        }

        private ExchangeRateDto TryReciprocalConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Currency fixedCurrency,
            Currency variableCurrency)
        {
            var reciprocalConversion = exchangeRates.FirstOrDefault(
                er => string.Equals(er.FixedCurrency, fixedCurrency.Code, StringComparison.InvariantCultureIgnoreCase)
                      && string.Equals(
                          er.VariableCurrency,
                          variableCurrency.Code,
                          StringComparison.InvariantCultureIgnoreCase));

            if (reciprocalConversion == null) return null;

            var reciprocalRate =

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                reciprocalConversion.Rate != 0 ? 1 / (decimal)reciprocalConversion.Rate : 0;

            return new ExchangeRateDto
                       {
                           FixedCurrency = fixedCurrency.Code,
                           VariableCurrency = variableCurrency.Code,
                           Rate = (double)reciprocalRate,
                           DateTime = reciprocalConversion.DateTime
                       };
        }
    }
}