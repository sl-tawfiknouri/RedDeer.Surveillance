﻿// ReSharper disable MemberCanBeMadeStatic.Local

namespace Surveillance.Engine.Rules.Currency
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
    ///     Performs similar work to exchange rates but also converts the underlying currency amounts
    /// </summary>
    public class CurrencyConverterService : ICurrencyConverterService
    {
        private readonly IExchangeRateApiCachingDecorator _exchangeRateApi;

        private readonly ILogger<CurrencyConverterService> _logger;

        public CurrencyConverterService(
            IExchangeRateApiCachingDecorator exchangeRateApi,
            ILogger<CurrencyConverterService> logger)
        {
            this._exchangeRateApi = exchangeRateApi ?? throw new ArgumentNullException(nameof(exchangeRateApi));

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Money?> Convert(
            IReadOnlyCollection<Money> monies,
            Currency targetCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            if (monies == null || !monies.Any())
            {
                this._logger.LogInformation(
                    $"received null or empty currency amounts. Returning 0 currency amount in target currency of {targetCurrency} for rule {ruleCtx?.Id()}");
                return new Money(0, targetCurrency);
            }

            if (string.IsNullOrWhiteSpace(targetCurrency.Code))
            {
                this._logger.LogError("asked to convert to a null or empty currency");
                return monies.Aggregate((i, o) => new Money(i.Value + o.Value, i.Currency));
            }

            if (monies.All(ca => Equals(ca.Currency, targetCurrency)))
            {
                this._logger.LogInformation(
                    "inferred all currency amounts matched the target currency. Aggregating trades and returning.");
                return monies.Aggregate((i, o) => new Money(i.Value + o.Value, i.Currency));
            }

            this._logger.LogInformation($"about to fetch exchange rates on {dayOfConversion}");
            var rates = await this.ExchangeRates(dayOfConversion, ruleCtx);

            if (rates == null || !rates.Any())
            {
                this._logger.LogError(
                    $"unable to change rates to {targetCurrency.Code} on {dayOfConversion.ToShortDateString()} due to missing rates");
                ruleCtx.EventException(
                    $"unable to change rates to {targetCurrency.Code} on {dayOfConversion.ToShortDateString()} due to missing rates");

                return null;
            }

            var convertedToTargetCurrency = monies.Select(
                currency => this.Convert(rates, currency, targetCurrency, dayOfConversion, ruleCtx)).ToList();

            var totalInConvertedCurrency = convertedToTargetCurrency.Where(cc => cc.HasValue).Select(cc => cc.Value)
                .Sum(cc => cc.Value);

            this._logger.LogInformation($"returning {totalInConvertedCurrency} ({targetCurrency})");
            return new Money(totalInConvertedCurrency, targetCurrency);
        }

        private Money? Convert(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Money initialMoney,
            Currency targetCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            if (Equals(initialMoney.Currency, targetCurrency))
            {
                this._logger.LogInformation(
                    $"asked to convert {initialMoney.Currency} to {targetCurrency} and found they were the same. Returning initial amount.");
                return initialMoney;
            }

            // direct exchange rate i.e. we want to do USD to GBP and we have USD / GBP
            var directConversion = this.TryDirectConversion(exchangeRates, initialMoney, targetCurrency);

            if (directConversion != null)
            {
                this._logger.LogInformation(
                    $"managed to directly convert {initialMoney.Currency} {initialMoney.Value} to {targetCurrency} {directConversion.Value.Value}.");
                return directConversion;
            }

            this._logger.LogInformation(
                $"failed to directly convert {initialMoney.Currency} to {targetCurrency}. Trying reciprocal conversion.");

            // reciprocal exchange rate i.e. we want to do USD to GBP but we have GBP / USD
            var reciprocalConversion = this.TryReciprocalConversion(exchangeRates, initialMoney, targetCurrency);

            if (reciprocalConversion != null)
            {
                this._logger.LogInformation(
                    $"managed to reciprocally convert {initialMoney.Currency} {initialMoney.Value} to {targetCurrency} {reciprocalConversion.Value.Value}.");
                return reciprocalConversion;
            }

            this._logger.LogInformation(
                $"failed to reciprocally convert {initialMoney.Currency} to {targetCurrency}. Trying indirect conversion.");

            // implicit exchange rate i.e. we want to do EUR to GBP but we have EUR / USD and GBP / USD
            var indirectConversion = this.TryIndirectConversion(
                exchangeRates,
                initialMoney,
                targetCurrency,
                dayOfConversion,
                ruleCtx);

            if (indirectConversion == null)
            {
                this._logger.LogError(
                    $"was unable to convert {initialMoney.Currency.Code} to {targetCurrency.Code} on {dayOfConversion} after attempting an indirect conversion. Returning null.");
                ruleCtx.EventException(
                    $"was unable to convert {initialMoney.Currency.Code} to {targetCurrency.Code} on {dayOfConversion}");

                return null;
            }

            this._logger.LogInformation(
                $"managed to indirectly convert {initialMoney.Currency} {initialMoney.Value} to {targetCurrency} {indirectConversion.Value.Value}.");

            return indirectConversion;
        }

        private async Task<IReadOnlyCollection<ExchangeRateDto>> ExchangeRates(
            DateTime dayOfRate,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            dayOfRate = dayOfRate.Date;
            var exchRate = await this._exchangeRateApi.GetAsync(dayOfRate, dayOfRate);

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
                this._logger.LogError($"could not find an exchange rate in a 14 day date range around {dayOfRate}.");
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

        private Money? TryDirectConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Money initialMoney,
            Currency targetCurrency)
        {
            var directConversion = exchangeRates.FirstOrDefault(
                er => string.Equals(
                          er.FixedCurrency,
                          initialMoney.Currency.Code,
                          StringComparison.InvariantCultureIgnoreCase) && string.Equals(
                          er.VariableCurrency,
                          targetCurrency.Code,
                          StringComparison.InvariantCultureIgnoreCase));

            if (directConversion == null) return null;

            var money = new Money((decimal)directConversion.Rate * initialMoney.Value, targetCurrency);

            return money;
        }

        private Money? TryIndirectConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Money initialMoney,
            Currency targetCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            var initialExchangeRate = this.GetExchangeRates(exchangeRates, initialMoney.Currency);
            var targetExchangeRate = this.GetExchangeRates(exchangeRates, targetCurrency);

            var sharedVariableRateInitial = initialExchangeRate.FirstOrDefault(
                ier => targetExchangeRate.Select(ter => ter.VariableCurrency).Contains(ier.VariableCurrency));

            if (sharedVariableRateInitial == null)
            {
                this._logger.LogError(
                    $"could not find a shared common currency using a one step approach for {initialMoney.Currency.Code} and {targetCurrency.Code} on {dayOfConversion}");

                ruleCtx.EventException(
                    $"could not find a shared common currency using a one step approach for {initialMoney.Currency.Code} and {targetCurrency.Code} on {dayOfConversion}");

                return null;
            }

            var sharedVariableRateTarget = targetExchangeRate.FirstOrDefault(
                tec => string.Equals(
                    tec.VariableCurrency,
                    sharedVariableRateInitial.VariableCurrency,
                    StringComparison.InvariantCultureIgnoreCase));

            if (sharedVariableRateTarget == null)
            {
                this._logger.LogError(
                    $"could not find a shared common currency using a one step approach for {initialMoney.Currency.Code} and {targetCurrency.Code} on {dayOfConversion}");

                ruleCtx.EventException(
                    $"could not find a shared common currency using a one step approach for {initialMoney.Currency.Code} and {targetCurrency.Code} on {dayOfConversion}");

                return null;
            }

            var variableCurrencyInitial = new Money(
                initialMoney.Value * (decimal)sharedVariableRateInitial.Rate,
                sharedVariableRateInitial.VariableCurrency);

            var reciprocalExchangeRate =

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                sharedVariableRateTarget.Rate != 0 ? 1 / (decimal)sharedVariableRateTarget.Rate : 0;

            var fixedTargetCurrencyInitial = new Money(
                variableCurrencyInitial.Value * reciprocalExchangeRate,
                targetCurrency);

            return fixedTargetCurrencyInitial;
        }

        private Money? TryReciprocalConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Money initialMoney,
            Currency targetCurrency)
        {
            var reciprocalConversion = exchangeRates.FirstOrDefault(
                er => string.Equals(er.FixedCurrency, targetCurrency.Code, StringComparison.InvariantCultureIgnoreCase)
                      && string.Equals(
                          er.VariableCurrency,
                          initialMoney.Currency.Code,
                          StringComparison.InvariantCultureIgnoreCase));

            if (reciprocalConversion == null) return null;

            var reciprocalRate =

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                reciprocalConversion.Rate != 0 ? 1 / (decimal)reciprocalConversion.Rate : 0;

            return new Money(reciprocalRate * initialMoney.Value, targetCurrency);
        }
    }
}