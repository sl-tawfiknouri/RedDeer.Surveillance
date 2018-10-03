﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Finance;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.Currency.Interfaces;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
// ReSharper disable MemberCanBeMadeStatic.Local

namespace Surveillance.Currency
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly IExchangeRateApiCachingDecoratorRepository _exchangeRateApiRepository;
        private readonly ILogger<CurrencyConverter> _logger;

        public CurrencyConverter(
            IExchangeRateApiCachingDecoratorRepository exchangeRateApiRepository,
            ILogger<CurrencyConverter> logger)
        {
            _exchangeRateApiRepository =
                exchangeRateApiRepository
                ?? throw new ArgumentNullException(nameof(exchangeRateApiRepository));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CurrencyAmount?> Convert(
            IReadOnlyCollection<CurrencyAmount> currencyAmounts,
            Domain.Finance.Currency targetCurrency,
            DateTime dayOfConversion)
        {
            if (currencyAmounts == null
                || !currencyAmounts.Any())
            {
                return null;
            }

            if (currencyAmounts.All(ca => Equals(ca.Currency, targetCurrency)))
            {
                return currencyAmounts.Aggregate((i,o) => new CurrencyAmount(i.Value + o.Value, i.Currency));
            }

            var rates = await ExchangeRates(dayOfConversion);

            if (rates == null
                || !rates.Any())
            {
                _logger.LogError($"Currency Converter unable to change rates to {targetCurrency} on {dayOfConversion.ToShortDateString()}");

                return null;
            }

            var convertedToTargetCurrency =
                currencyAmounts
                    .Select(currency => Convert(rates, currency, targetCurrency, dayOfConversion))
                    .ToList();

            var totalInConvertedCurrency = convertedToTargetCurrency
                .Where(cc => cc.HasValue)
                .Select(cc => cc.Value)
                .Sum(cc => cc.Value);

            return new CurrencyAmount(totalInConvertedCurrency, targetCurrency);
        }

        private CurrencyAmount? Convert(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            CurrencyAmount initial,
            Domain.Finance.Currency targetCurrency,
            DateTime dayOfConversion)
        {
            if (Equals(initial.Currency, targetCurrency))
            {
                return initial;
            }

            // direct exchange rate i.e. we want to do USD to GBP and we have USD / GBP
            var directConversion = TryDirectConversion(exchangeRates, initial, targetCurrency);

            if (directConversion != null)
            {
                return directConversion;
            }

            // reciprocal exchange rate i.e. we want to do USD to GBP but we have GBP / USD
            var reciprocalConversion = TryReciprocalConversion(exchangeRates, initial, targetCurrency);

            if (reciprocalConversion != null)
            {
                return reciprocalConversion;
            }

            // implicit exchange rate i.e. we want to do EUR to GBP but we have EUR / USD and GBP / USD
            var indirectConversion = TryIndirectConversion(exchangeRates, initial, targetCurrency, dayOfConversion);

            if (indirectConversion == null)
            {
                _logger.LogError($"Currency Converter was unable to convert {initial.Currency.Value} to {targetCurrency.Value} on {dayOfConversion}");
            }

            return indirectConversion;
        }

        private CurrencyAmount? TryDirectConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            CurrencyAmount initial,
            Domain.Finance.Currency targetCurrency)
        {
            var directConversion = exchangeRates
                .FirstOrDefault(er =>
                    string.Equals(er.FixedCurrency, initial.Currency.Value, StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(er.VariableCurrency, targetCurrency.Value, StringComparison.InvariantCultureIgnoreCase));

            if (directConversion == null)
            {
                return null;
            }

            var currencyAmount = new CurrencyAmount((decimal) directConversion.Rate * initial.Value, targetCurrency);

            return currencyAmount;
        }
        
        private CurrencyAmount? TryReciprocalConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            CurrencyAmount initial,
            Domain.Finance.Currency targetCurrency)
        {
            var reciprocalConversion = exchangeRates
                .FirstOrDefault(er =>
                    string.Equals(er.FixedCurrency, targetCurrency.Value, StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(er.VariableCurrency, initial.Currency.Value, StringComparison.InvariantCultureIgnoreCase));

            if (reciprocalConversion == null)
            {
                return null;
            }

            var reciprocalRate =
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                reciprocalConversion.Rate != 0
                    ? (decimal)1 / (decimal)reciprocalConversion.Rate
                    : 0;

            return new CurrencyAmount(reciprocalRate * initial.Value, targetCurrency);
        }

        private CurrencyAmount? TryIndirectConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            CurrencyAmount initial,
            Domain.Finance.Currency targetCurrency,
            DateTime dayOfConversion)
        {
            var initialExchangeRate = GetExchangeRates(exchangeRates, initial.Currency);
            var targetExchangeRate = GetExchangeRates(exchangeRates, targetCurrency);

            var sharedVariableRateInitial = initialExchangeRate
                .FirstOrDefault(ier =>
                    targetExchangeRate
                        .Select(ter => ter.VariableCurrency).Contains(ier.VariableCurrency));

            if (sharedVariableRateInitial == null)
            {
                _logger.LogError($"Currency Converter could not find a shared common currency using a one step approach for {initial.Currency} and {targetCurrency.Value} on {dayOfConversion}");

                return null;
            }

            var sharedVariableRateTarget =
                targetExchangeRate
                    .FirstOrDefault(tec =>
                        string.Equals(
                            tec.VariableCurrency,
                            sharedVariableRateInitial.VariableCurrency,
                            StringComparison.InvariantCultureIgnoreCase));

            if (sharedVariableRateTarget == null)
            {
                _logger.LogError($"Currency Converter could not find a shared common currency using a one step approach for {initial.Currency} and {targetCurrency.Value} on {dayOfConversion}");

                return null;
            }

            var variableCurrencyInitial =
                new CurrencyAmount(
                    initial.Value * (decimal)sharedVariableRateInitial.Rate,
                    sharedVariableRateInitial.VariableCurrency);

            var reciprocalExchangeRate =
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                sharedVariableRateTarget.Rate != 0
                ? (decimal)1 / (decimal)sharedVariableRateTarget.Rate
                : 0;

            var fixedTargetCurrencyInitial =
                new CurrencyAmount(
                    variableCurrencyInitial.Value * reciprocalExchangeRate,
                    targetCurrency);

            return fixedTargetCurrencyInitial;
        }

        private List<ExchangeRateDto> GetExchangeRates(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Domain.Finance.Currency targetCurrency)
        {
            return exchangeRates
                .Where(er =>
                    string.Equals(
                        er.FixedCurrency,
                        targetCurrency.Value,
                        StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<IReadOnlyCollection<ExchangeRateDto>> ExchangeRates(DateTime dayOfRate)
        {
            dayOfRate = dayOfRate.Date;
            var exchRate = await _exchangeRateApiRepository.Get(dayOfRate, dayOfRate);

            // cycle through last two weeks of exchange rates
            var offset = 0;
            var cycleDate = dayOfRate;
            while (!exchRate.ContainsKey(cycleDate)
                   && offset < 15)
            {
                offset += 1;
                cycleDate = cycleDate.AddDays(-1);
            }

            if (offset > 14)
            {
                _logger.LogError($"High Profit Rule could not find an exchange rate in the date range around {dayOfRate}.");

                return new ExchangeRateDto[0];
            }

            if (!exchRate.TryGetValue(cycleDate, out var rates))
            {
                _logger.LogError($"High Profit Rule could not find an exchange rate in the date range around {dayOfRate} in the dictionary.");

                return new ExchangeRateDto[0];
            }

            return rates;
        }
    }
}