using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Core.Financial.Money;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;

// ReSharper disable MemberCanBeMadeStatic.Local
namespace Surveillance.Engine.Rules.Currency
{
    /// <summary>
    /// Performs similar work to exchange rates but also converts the underlying currency amounts
    /// </summary>
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

        public async Task<Money?> Convert(
            IReadOnlyCollection<Money> monies,
            Domain.Core.Financial.Money.Currency targetCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            if (monies == null
                || !monies.Any())
            {
                _logger.LogInformation($"CurrencyConverter received null or empty currency amounts. Returning 0 currency amount in target currency of {targetCurrency} for rule {ruleCtx?.Id()}");
                return new Money(0, targetCurrency);
            }

            if (string.IsNullOrWhiteSpace(targetCurrency.Code))
            {
                _logger.LogError($"CurrencyConverter asked to convert to a null or empty currency");
                return monies.Aggregate((i, o) => new Money(i.Value + o.Value, i.Currency));
            }

            if (monies.All(ca => Equals(ca.Currency, targetCurrency)))
            {
                _logger.LogInformation($"CurrencyConverter inferred all currency amounts matched the target currency. Aggregating trades and returning.");
                return monies.Aggregate((i,o) => new Money(i.Value + o.Value, i.Currency));
            }

            _logger.LogInformation($"CurrencyConverter about to fetch exchange rates on {dayOfConversion}");
            var rates = await ExchangeRates(dayOfConversion, ruleCtx);

            if (rates == null
                || !rates.Any())
            {
                _logger.LogError($"Currency Converter unable to change rates to {targetCurrency.Code} on {dayOfConversion.ToShortDateString()} due to missing rates");
                ruleCtx.EventException($"Currency Converter unable to change rates to {targetCurrency.Code} on {dayOfConversion.ToShortDateString()} due to missing rates");

                return null;
            }

            var convertedToTargetCurrency =
                monies
                    .Select(currency => Convert(rates, currency, targetCurrency, dayOfConversion, ruleCtx))
                    .ToList();

            var totalInConvertedCurrency = convertedToTargetCurrency
                .Where(cc => cc.HasValue)
                .Select(cc => cc.Value)
                .Sum(cc => cc.Value);

            _logger.LogInformation($"CurrencyConverter returning {totalInConvertedCurrency} ({targetCurrency})");
            return new Money(totalInConvertedCurrency, targetCurrency);
        }

        private Money? Convert(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Money initialMoney,
            Domain.Core.Financial.Money.Currency targetCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            if (Equals(initialMoney.Currency, targetCurrency))
            {
                _logger.LogInformation($"CurrencyConverter asked to convert {initialMoney.Currency} to {targetCurrency} and found they were the same. Returning initial amount.");
                return initialMoney;
            }

            // direct exchange rate i.e. we want to do USD to GBP and we have USD / GBP
            var directConversion = TryDirectConversion(exchangeRates, initialMoney, targetCurrency);

            if (directConversion != null)
            {
                _logger.LogInformation($"CurrencyConverter managed to directly convert {initialMoney.Currency} {initialMoney.Value} to {targetCurrency} {directConversion.Value.Value}.");
                return directConversion;
            }

            _logger.LogInformation($"CurrencyConverter failed to directly convert {initialMoney.Currency} to {targetCurrency}. Trying reciprocal conversion.");
            // reciprocal exchange rate i.e. we want to do USD to GBP but we have GBP / USD
            var reciprocalConversion = TryReciprocalConversion(exchangeRates, initialMoney, targetCurrency);

            if (reciprocalConversion != null)
            {
                _logger.LogInformation($"CurrencyConverter managed to reciprocally convert {initialMoney.Currency} {initialMoney.Value} to {targetCurrency} {reciprocalConversion.Value.Value}.");
                return reciprocalConversion;
            }

            _logger.LogInformation($"CurrencyConverter failed to reciprocally convert {initialMoney.Currency} to {targetCurrency}. Trying indirect conversion.");
            // implicit exchange rate i.e. we want to do EUR to GBP but we have EUR / USD and GBP / USD
            var indirectConversion = TryIndirectConversion(exchangeRates, initialMoney, targetCurrency, dayOfConversion, ruleCtx);

            if (indirectConversion == null)
            {
                _logger.LogError($"Currency Converter was unable to convert {initialMoney.Currency.Code} to {targetCurrency.Code} on {dayOfConversion} after attempting an indirect conversion. Returning null.");
                ruleCtx.EventException($"Currency Converter was unable to convert {initialMoney.Currency.Code} to {targetCurrency.Code} on {dayOfConversion}");

                return null;
            }

            _logger.LogInformation($"CurrencyConverter managed to indirectly convert {initialMoney.Currency} {initialMoney.Value} to {targetCurrency} {indirectConversion.Value.Value}.");

            return indirectConversion;
        }

        private Money? TryDirectConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Money initialMoney,
            Domain.Core.Financial.Money.Currency targetCurrency)
        {
            var directConversion = exchangeRates
                .FirstOrDefault(er =>
                    string.Equals(er.FixedCurrency, initialMoney.Currency.Code, StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(er.VariableCurrency, targetCurrency.Code, StringComparison.InvariantCultureIgnoreCase));

            if (directConversion == null)
            {
                return null;
            }

            var money = new Money((decimal) directConversion.Rate * initialMoney.Value, targetCurrency);

            return money;
        }
        
        private Money? TryReciprocalConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Money initialMoney,
            Domain.Core.Financial.Money.Currency targetCurrency)
        {
            var reciprocalConversion = exchangeRates
                .FirstOrDefault(er =>
                    string.Equals(er.FixedCurrency, targetCurrency.Code, StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(er.VariableCurrency, initialMoney.Currency.Code, StringComparison.InvariantCultureIgnoreCase));

            if (reciprocalConversion == null)
            {
                return null;
            }

            var reciprocalRate =
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                reciprocalConversion.Rate != 0
                    ? (decimal)1 / (decimal)reciprocalConversion.Rate
                    : 0;

            return new Money(reciprocalRate * initialMoney.Value, targetCurrency);
        }

        private Money? TryIndirectConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Money initialMoney,
            Domain.Core.Financial.Money.Currency targetCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            var initialExchangeRate = GetExchangeRates(exchangeRates, initialMoney.Currency);
            var targetExchangeRate = GetExchangeRates(exchangeRates, targetCurrency);

            var sharedVariableRateInitial = initialExchangeRate
                .FirstOrDefault(ier =>
                    targetExchangeRate
                        .Select(ter => ter.VariableCurrency).Contains(ier.VariableCurrency));

            if (sharedVariableRateInitial == null)
            {
                _logger.LogError($"Currency Converter could not find a shared common currency using a one step approach for {initialMoney.Currency.Code} and {targetCurrency.Code} on {dayOfConversion}");

                ruleCtx.EventException($"Currency Converter could not find a shared common currency using a one step approach for {initialMoney.Currency.Code} and {targetCurrency.Code} on {dayOfConversion}");

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
                _logger.LogError($"Currency Converter could not find a shared common currency using a one step approach for {initialMoney.Currency.Code} and {targetCurrency.Code} on {dayOfConversion}");

                ruleCtx.EventException($"Currency Converter could not find a shared common currency using a one step approach for {initialMoney.Currency.Code} and {targetCurrency.Code} on {dayOfConversion}");

                return null;
            }

            var variableCurrencyInitial =
                new Money(
                    initialMoney.Value * (decimal)sharedVariableRateInitial.Rate,
                    sharedVariableRateInitial.VariableCurrency);

            var reciprocalExchangeRate =
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                sharedVariableRateTarget.Rate != 0
                ? (decimal)1 / (decimal)sharedVariableRateTarget.Rate
                : 0;

            var fixedTargetCurrencyInitial =
                new Money(
                    variableCurrencyInitial.Value * reciprocalExchangeRate,
                    targetCurrency);

            return fixedTargetCurrencyInitial;
        }

        private List<ExchangeRateDto> GetExchangeRates(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Domain.Core.Financial.Money.Currency targetCurrency)
        {
            return exchangeRates
                .Where(er =>
                    string.Equals(
                        er.FixedCurrency,
                        targetCurrency.Code,
                        StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<IReadOnlyCollection<ExchangeRateDto>> ExchangeRates(
            DateTime dayOfRate,
            ISystemProcessOperationRunRuleContext ruleCtx)
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
                _logger.LogError($"High Profit Rule could not find an exchange rate in a 14 day date range around {dayOfRate}.");
                ruleCtx.EventException($"High Profit Rule could not find an exchange rate in the date range around {dayOfRate}.");

                return new ExchangeRateDto[0];
            }

            if (!exchRate.TryGetValue(cycleDate, out var rates))
            {
                _logger.LogError($"High Profit Rule could not find an exchange rate in the date range around {dayOfRate} in the dictionary.");
                ruleCtx.EventException($"High Profit Rule could not find an exchange rate in the date range around {dayOfRate} in the dictionary.");

                return new ExchangeRateDto[0];
            }

            return rates;
        }
    }
}