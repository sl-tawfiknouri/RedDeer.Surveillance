using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;

namespace Surveillance.Engine.Rules.Currency
{
    /// <summary>
    /// Will fetch the correct x-rate for a pair of currencies on a given date
    ///
    /// The quotation EUR/USD 1.3225 means that 1 Euro will buy 1.3225 US dollars.
    /// In other words, this is the price of a unit of Euro in US dollars.
    /// Here, EUR is called the "Fixed currency", while USD is called the "Variable currency".
    /// </summary>
    public class ExchangeRates : IExchangeRates
    {
        private readonly IExchangeRateApiCachingDecoratorRepository _exchangeRateApiRepository;
        private readonly ILogger<ExchangeRates> _logger;

        public ExchangeRates(
            IExchangeRateApiCachingDecoratorRepository exchangeRateApiRepository,
            ILogger<ExchangeRates> logger)
        {
            _exchangeRateApiRepository =
                exchangeRateApiRepository
                ?? throw new ArgumentNullException(nameof(exchangeRateApiRepository));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // fixed is basically from
        // variable is to
        // so EUR/USD 1.3225 means 1 euro buys 1.3225 dollars
        // with eur = fixed and usd = variable currencies
        public async Task<ExchangeRateDto> GetRate(
            Domain.Core.Financial.Currency fixedCurrency,
            Domain.Core.Financial.Currency variableCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            if (string.IsNullOrWhiteSpace(fixedCurrency.Code)
                || string.IsNullOrWhiteSpace(variableCurrency.Code))
            {
                _logger.LogError($"ExchangeRate was asked to convert two currencies. Once of which was null or empty {fixedCurrency} {variableCurrency}");
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

                _logger.LogInformation($"ExchangeRate was asked to convert two currencies but they were equal. Returning a rate of 1 for {fixedCurrency} and {variableCurrency}");

                return noConversionRate;
            }

            var rates = await GetExchangeRatesNearestToDate(dayOfConversion, ruleCtx);

            if (rates == null
                || !rates.Any())
            {
                _logger.LogError($"ExchangeRates unable to find any rates on {dayOfConversion.ToShortDateString()}");
                ruleCtx.EventException($"ExchangeRates unable to change rates from {fixedCurrency.Code} to {variableCurrency.Code} on {dayOfConversion.ToShortDateString()}");

                return null;
            }

            var rate = Convert(rates, fixedCurrency, variableCurrency, dayOfConversion, ruleCtx);

            _logger.LogInformation($"ExchangeRate was asked to convert two currencies {fixedCurrency} and {variableCurrency} on {dayOfConversion}. Returning {rate.Rate} as the exchange rate");

            return rate;
        }

        private ExchangeRateDto Convert(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Domain.Core.Financial.Currency fixedCurrency,
            Domain.Core.Financial.Currency variableCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            // direct exchange rate i.e. we want to do USD to GBP and we have USD / GBP
            var directConversion = TryDirectConversion(exchangeRates, fixedCurrency, variableCurrency);

            if (directConversion != null)
            {
                _logger.LogInformation($"ExchangeRates was able to directly convert {fixedCurrency} to {variableCurrency} at rate of {directConversion.Rate} on {directConversion.DateTime}");

                return directConversion;
            }

            // reciprocal exchange rate i.e. we want to do USD to GBP but we have GBP / USD
            var reciprocalConversion = TryReciprocalConversion(exchangeRates, fixedCurrency, variableCurrency);

            if (reciprocalConversion != null)
            {
                _logger.LogInformation($"ExchangeRates was able to reciprocally convert {fixedCurrency} to {variableCurrency} at rate of {reciprocalConversion.Rate} on {reciprocalConversion.DateTime}");

                return reciprocalConversion;
            }

            // implicit exchange rate i.e. we want to do EUR to GBP but we have EUR / USD and GBP / USD
            var indirectConversion = TryIndirectConversion(exchangeRates, fixedCurrency, variableCurrency, dayOfConversion, ruleCtx);

            if (indirectConversion == null)
            {
                _logger.LogError($"Exchange Rates was unable to convert {fixedCurrency.Code} to {variableCurrency.Code} on {dayOfConversion}");
                ruleCtx.EventException($"Exchange Rates was unable to convert {fixedCurrency.Code} to {variableCurrency.Code} on {dayOfConversion}");

                return null;
            }

            _logger.LogInformation($"ExchangeRates was able to indirectly convert {fixedCurrency} to {variableCurrency} at rate of {indirectConversion.Rate} on {indirectConversion.DateTime}");

            return indirectConversion;
        }

        private ExchangeRateDto TryDirectConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Domain.Core.Financial.Currency fixedCurrency,
            Domain.Core.Financial.Currency variableCurrency)
        {
            var directConversion = exchangeRates
                .FirstOrDefault(er =>
                    string.Equals(er.FixedCurrency, fixedCurrency.Code, StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(er.VariableCurrency, variableCurrency.Code, StringComparison.InvariantCultureIgnoreCase));

            return directConversion;
        }

        private ExchangeRateDto TryReciprocalConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Domain.Core.Financial.Currency fixedCurrency,
            Domain.Core.Financial.Currency variableCurrency)
        {
            var reciprocalConversion = exchangeRates
                .FirstOrDefault(er =>
                    string.Equals(er.FixedCurrency, fixedCurrency.Code, StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(er.VariableCurrency, variableCurrency.Code, StringComparison.InvariantCultureIgnoreCase));

            if (reciprocalConversion == null)
            {
                return null;
            }

            var reciprocalRate =
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                reciprocalConversion.Rate != 0
                    ? (decimal)1 / (decimal)reciprocalConversion.Rate
                    : 0;

            return new ExchangeRateDto
            {
                FixedCurrency = fixedCurrency.Code,
                VariableCurrency = variableCurrency.Code,
                Rate = (double)reciprocalRate,
                DateTime = reciprocalConversion.DateTime
            };
        }

        private ExchangeRateDto TryIndirectConversion(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Domain.Core.Financial.Currency fixedCurrency,
            Domain.Core.Financial.Currency variableCurrency,
            DateTime dayOfConversion,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            var fixedExchangeRates = GetExchangeRates(exchangeRates, fixedCurrency);
            var variableExchangeRates = GetExchangeRates(exchangeRates, variableCurrency);

            var sharedVariableRateInitial = fixedExchangeRates
                .FirstOrDefault(ier =>
                    variableExchangeRates
                        .Select(ter => ter.VariableCurrency).Contains(ier.VariableCurrency));

            if (sharedVariableRateInitial == null)
            {
                _logger.LogError($"Exchange Rates could not find a shared common currency using a one step approach for {fixedCurrency.Code} and {variableCurrency.Code} on {dayOfConversion}");

                ruleCtx.EventException($"Exchange Rates could not find a shared common currency using a one step approach for {fixedCurrency.Code} and {variableCurrency.Code} on {dayOfConversion}");

                return null;
            }

            var sharedVariableRateTarget =
                variableExchangeRates
                    .FirstOrDefault(tec =>
                        string.Equals(
                            tec.VariableCurrency,
                            sharedVariableRateInitial.VariableCurrency,
                            StringComparison.InvariantCultureIgnoreCase));

            if (sharedVariableRateTarget == null)
            {
                _logger.LogError($"Exchange Rates could not find a shared common currency using a one step approach for {fixedCurrency.Code} and {variableCurrency.Code} on {dayOfConversion}");

                ruleCtx.EventException($"Exchange Rates could not find a shared common currency using a one step approach for {fixedCurrency.Code} and {variableCurrency.Code} on {dayOfConversion}");

                return null;
            }
            
            var reciprocalExchangeRate =
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                sharedVariableRateTarget.Rate != 0
                ? (decimal)1 / (decimal)sharedVariableRateTarget.Rate
                : 0;

            var completeRate = (double)reciprocalExchangeRate * sharedVariableRateInitial.Rate;

            return new ExchangeRateDto
            {
                DateTime = sharedVariableRateTarget.DateTime,
                FixedCurrency = fixedCurrency.Code,
                VariableCurrency = variableCurrency.Code,
                Rate = completeRate
            };
        }

        private List<ExchangeRateDto> GetExchangeRates(
            IReadOnlyCollection<ExchangeRateDto> exchangeRates,
            Domain.Core.Financial.Currency targetCurrency)
        {
            return exchangeRates
                .Where(er =>
                    string.Equals(
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
                _logger.LogError($"Exchange Rates could not find an exchange rate in the date range around {dayOfRate}.");
                ruleCtx.EventException($"Exchange Rates could not find an exchange rate in the date range around {dayOfRate}.");

                return new ExchangeRateDto[0];
            }

            if (!exchRate.TryGetValue(cycleDate, out var rates))
            {
                _logger.LogError($"Exchange Rates could not find an exchange rate in the date range around {dayOfRate} in the dictionary.");
                ruleCtx.EventException($"Exchange Rates could not find an exchange rate in the date range around {dayOfRate} in the dictionary.");

                return new ExchangeRateDto[0];
            }

            return rates;
        }

    }
}
