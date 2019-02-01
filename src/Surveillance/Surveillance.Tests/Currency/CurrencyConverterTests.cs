using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Financial;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.Currency;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Tests.Currency
{
    [TestFixture]
    public class CurrencyConverterTests
    {
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IExchangeRateApiCachingDecoratorRepository _apiRepository;
        private ILogger<CurrencyConverter> _logger;
        private DomainV2.Financial.Currency _currency;
        private DateTime _conversionTime;

        [SetUp]
        public void Setup()
        {
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _apiRepository = A.Fake<IExchangeRateApiCachingDecoratorRepository>();
            _logger = A.Fake<ILogger<CurrencyConverter>>();
            _currency = new DomainV2.Financial.Currency("USD");
            _conversionTime = new DateTime(2017, 8, 31);
        }

        [Test]
        public void Constructor_ThrowsForNull_Repository()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CurrencyConverter(null, _logger));
        }

        [Test]
        public void Constructor_ThrowsForNull_Logger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CurrencyConverter(_apiRepository, null));
        }

        [Test]
        public async Task Convert_NullCurrencyAmounts_ReturnsNotNull()
        {
            var converter = new CurrencyConverter(_apiRepository, _logger);

            var result = await converter.Convert(null, _currency, _conversionTime, _ruleCtx);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Convert_EmptyCurrencyAmounts_ReturnsNotNull()
        {
            var converter = new CurrencyConverter(_apiRepository, _logger);
            var currencyAmounts = new List<CurrencyAmount>();

            var result = await converter.Convert(currencyAmounts, _currency, _conversionTime, _ruleCtx);
            
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Convert_InSameCurrencyAsTarget_DoesNotCallExchangeRateApi()
        {
            var converter = new CurrencyConverter(_apiRepository, _logger);
            var currencyAmounts = new List<CurrencyAmount>
            {
                new CurrencyAmount(10, "CNY")
            };
            var targetCurrency = new DomainV2.Financial.Currency("CNY");

            var conversion = await converter.Convert(currencyAmounts, targetCurrency, DateTime.UtcNow, _ruleCtx);

            Assert.AreEqual(conversion.Value.Value, 10);
            Assert.AreEqual(conversion.Value.Currency.Value, "CNY");

            A.CallTo(() => _apiRepository.Get(A<DateTime>.Ignored, A<DateTime>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public async Task Convert_NullRatesResult_ReturnsNull()
        {
            var converter = new CurrencyConverter(_apiRepository, _logger);
            var currencyAmounts = new List<CurrencyAmount>
            {
                new CurrencyAmount(100, "CNY")
            };
            var targetCurrency = new DomainV2.Financial.Currency("GBP");
            var targetDate = new DateTime(2018, 01, 01);

            var conversion = await converter.Convert(currencyAmounts, targetCurrency, targetDate, _ruleCtx);

            Assert.IsNull(conversion);
            A.CallTo(() => _apiRepository.Get(targetDate, targetDate)).MustHaveHappened();
        }

        [Test]
        public async Task Convert_EmptyRatesResult_ReturnsNull()
        {
            var converter = new CurrencyConverter(_apiRepository, _logger);
            var currencyAmounts = new List<CurrencyAmount>
            {
                new CurrencyAmount(100, "CNY")
            };
            var targetCurrency = new DomainV2.Financial.Currency("GBP");
            var targetDate = new DateTime(2018, 01, 01);

            A
                .CallTo(() => _apiRepository.Get(targetDate, targetDate))
                .Returns(new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>());

            var conversion = await converter.Convert(currencyAmounts, targetCurrency, targetDate, _ruleCtx);

            Assert.IsNull(conversion);
            A.CallTo(() => _apiRepository.Get(targetDate, targetDate)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Convert_WithDirectConversion_ReturnsExpectedResult()
        {
            var converter = new CurrencyConverter(_apiRepository, _logger);
            var currencyAmounts = new List<CurrencyAmount>
            {
                new CurrencyAmount(100, "CNY")
            };
            var targetCurrency = new DomainV2.Financial.Currency("GBP");
            var targetDate = new DateTime(2018, 01, 01);
            var rate = new ExchangeRateDto
            {
                DateTime = targetDate,
                FixedCurrency = "CNY",
                VariableCurrency = "GBP",
                Name = "Pound Sterling",
                Rate = 0.1
            };
            var rates = new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
            {
                {targetDate, new [] {rate}},
            };

            A
                .CallTo(() => _apiRepository.Get(targetDate, targetDate))
                .Returns(rates);

            var conversion = await converter.Convert(currencyAmounts, targetCurrency, targetDate, _ruleCtx);

            Assert.IsNotNull(conversion);
            Assert.AreEqual(conversion.Value.Value, 10);
            Assert.AreEqual(conversion.Value.Currency.Value, "GBP");
            A.CallTo(() => _apiRepository.Get(targetDate, targetDate)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Convert_WithDirectConversionMultipleRates_ReturnsExpectedResult()
        {
            var converter = new CurrencyConverter(_apiRepository, _logger);
            var currencyAmounts = new List<CurrencyAmount>
            {
                new CurrencyAmount(100, "CNY"),
                new CurrencyAmount(20, "USD")
            };
            var targetCurrency = new DomainV2.Financial.Currency("GBP");
            var targetDate = new DateTime(2018, 01, 01);
            var cnyRate = new ExchangeRateDto
            {
                DateTime = targetDate,
                FixedCurrency = "CNY",
                VariableCurrency = "GBP",
                Name = "Pound Sterling",
                Rate = 0.1
            };
            var usdRate = new ExchangeRateDto
            {
                DateTime = targetDate,
                FixedCurrency = "USD",
                VariableCurrency = "GBP",
                Name = "Pound Sterling",
                Rate = 0.5
            };

            var rates = new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
            {
                {targetDate, new [] {cnyRate, usdRate}},
            };

            A
                .CallTo(() => _apiRepository.Get(targetDate, targetDate))
                .Returns(rates);

            var conversion = await converter.Convert(currencyAmounts, targetCurrency, targetDate, _ruleCtx);

            Assert.IsNotNull(conversion);
            Assert.AreEqual(conversion.Value.Value, 20);
            Assert.AreEqual(conversion.Value.Currency.Value, "GBP");
            A.CallTo(() => _apiRepository.Get(targetDate, targetDate)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Convert_WithDirectConversionMultipleRatesButOneIsMissing_ReturnsExpectedResult()
        {
            var converter = new CurrencyConverter(_apiRepository, _logger);
            var currencyAmounts = new List<CurrencyAmount>
            {
                new CurrencyAmount(100, "CNY"),
                new CurrencyAmount(20, "USD")
            };
            var targetCurrency = new DomainV2.Financial.Currency("GBP");
            var targetDate = new DateTime(2018, 01, 01);
            var cnyRate = new ExchangeRateDto
            {
                DateTime = targetDate,
                FixedCurrency = "CNY",
                VariableCurrency = "GBP",
                Name = "Pound Sterling",
                Rate = 0.1
            };

            var rates = new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
            {
                {targetDate, new [] {cnyRate}},
            };

            A
                .CallTo(() => _apiRepository.Get(targetDate, targetDate))
                .Returns(rates);

            var conversion = await converter.Convert(currencyAmounts, targetCurrency, targetDate, _ruleCtx);

            Assert.IsNotNull(conversion);
            Assert.AreEqual(conversion.Value.Value, 10);
            Assert.AreEqual(conversion.Value.Currency.Value, "GBP");
            A.CallTo(() => _apiRepository.Get(targetDate, targetDate)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Convert_WithReciprocalConversion_ReturnsExpectedResult()
        {
            var converter = new CurrencyConverter(_apiRepository, _logger);
            var currencyAmounts = new List<CurrencyAmount>
            {
                new CurrencyAmount(100, "CNY"),
            };
            var targetCurrency = new DomainV2.Financial.Currency("GBP");
            var targetDate = new DateTime(2018, 01, 01);
            var cnyRate = new ExchangeRateDto
            {
                DateTime = targetDate,
                FixedCurrency = "GBP",
                VariableCurrency = "CNY",
                Name = "Pound Sterling",
                Rate = 10
            };

            var rates = new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
            {
                {targetDate, new [] {cnyRate}},
            };

            A
                .CallTo(() => _apiRepository.Get(targetDate, targetDate))
                .Returns(rates);

            var conversion = await converter.Convert(currencyAmounts, targetCurrency, targetDate, _ruleCtx);

            Assert.IsNotNull(conversion);
            Assert.AreEqual(conversion.Value.Value, 10);
            Assert.AreEqual(conversion.Value.Currency.Value, "GBP");
            A.CallTo(() => _apiRepository.Get(targetDate, targetDate)).MustHaveHappenedOnceExactly();
        }

        [TestCase(1, 1, 100)]
        [TestCase(0.1, 1, 10)]
        [TestCase(0.1, 0.1, 100)]
        [TestCase(1, 0.5, 200)]
        public async Task Convert_WithIndirectConversionRateSetOneRates_ReturnsExpectedResult(decimal rate1, decimal rate2, decimal expected)
        {
            var converter = new CurrencyConverter(_apiRepository, _logger);
            var currencyAmounts = new List<CurrencyAmount>
            {
                new CurrencyAmount(100, "CNY")
            };
            var targetCurrency = new DomainV2.Financial.Currency("EUR");
            var targetDate = new DateTime(2018, 01, 01);
            var cnyRate = new ExchangeRateDto
            {
                DateTime = targetDate,
                FixedCurrency = "CNY",
                VariableCurrency = "USD",
                Name = "Thaler",
                Rate = (double)rate1
            };
            var eurRate = new ExchangeRateDto
            {
                DateTime = targetDate,
                FixedCurrency = "EUR",
                VariableCurrency = "USD",
                Name = "Thaler",
                Rate = (double)rate2
            };

            var rates = new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
            {
                {targetDate, new [] {cnyRate, eurRate}},
            };

            A
                .CallTo(() => _apiRepository.Get(targetDate, targetDate))
                .Returns(rates);

            var conversion = await converter.Convert(currencyAmounts, targetCurrency, targetDate, _ruleCtx);

            Assert.IsNotNull(conversion);
            Assert.AreEqual(conversion.Value.Value, expected);
            Assert.AreEqual(conversion.Value.Currency.Value, "EUR");
            A.CallTo(() => _apiRepository.Get(targetDate, targetDate)).MustHaveHappenedOnceExactly();
        }
    }
}
