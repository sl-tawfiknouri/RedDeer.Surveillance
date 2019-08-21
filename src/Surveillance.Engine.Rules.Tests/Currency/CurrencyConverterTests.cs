namespace Surveillance.Engine.Rules.Tests.Currency
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Money;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Currency;
    using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;

    [TestFixture]
    public class CurrencyConverterTests
    {
        private IExchangeRateApiCachingDecorator _apiRepository;

        private DateTime _conversionTime;

        private Currency _currency;

        private ILogger<CurrencyConverterService> _logger;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        [Test]
        public void Constructor_ThrowsForNull_Logger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CurrencyConverterService(this._apiRepository, null));
        }

        [Test]
        public void Constructor_ThrowsForNull_Repository()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CurrencyConverterService(null, this._logger));
        }

        [Test]
        public async Task Convert_EmptyMoneys_ReturnsNotNull()
        {
            var converter = new CurrencyConverterService(this._apiRepository, this._logger);
            var Moneys = new List<Money>();

            var result = await converter.Convert(Moneys, this._currency, this._conversionTime, this._ruleCtx);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Convert_EmptyRatesResult_ReturnsNull()
        {
            var converter = new CurrencyConverterService(this._apiRepository, this._logger);
            var Moneys = new List<Money> { new Money(100, "CNY") };
            var targetCurrency = new Currency("GBP");
            var targetDate = new DateTime(2018, 01, 01);

            A.CallTo(() => this._apiRepository.Get(targetDate, targetDate))
                .Returns(new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>());

            var conversion = await converter.Convert(Moneys, targetCurrency, targetDate, this._ruleCtx);

            Assert.IsNull(conversion);
            A.CallTo(() => this._apiRepository.Get(targetDate, targetDate)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Convert_InSameCurrencyAsTarget_DoesNotCallExchangeRateApi()
        {
            var converter = new CurrencyConverterService(this._apiRepository, this._logger);
            var Moneys = new List<Money> { new Money(10, "CNY") };
            var targetCurrency = new Currency("CNY");

            var conversion = await converter.Convert(Moneys, targetCurrency, DateTime.UtcNow, this._ruleCtx);

            Assert.AreEqual(conversion.Value.Value, 10);
            Assert.AreEqual(conversion.Value.Currency.Code, "CNY");

            A.CallTo(() => this._apiRepository.Get(A<DateTime>.Ignored, A<DateTime>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public async Task Convert_NullMoneys_ReturnsNotNull()
        {
            var converter = new CurrencyConverterService(this._apiRepository, this._logger);

            var result = await converter.Convert(null, this._currency, this._conversionTime, this._ruleCtx);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Convert_NullRatesResult_ReturnsNull()
        {
            var converter = new CurrencyConverterService(this._apiRepository, this._logger);
            var Moneys = new List<Money> { new Money(100, "CNY") };
            var targetCurrency = new Currency("GBP");
            var targetDate = new DateTime(2018, 01, 01);

            var conversion = await converter.Convert(Moneys, targetCurrency, targetDate, this._ruleCtx);

            Assert.IsNull(conversion);
            A.CallTo(() => this._apiRepository.Get(targetDate, targetDate)).MustHaveHappened();
        }

        [Test]
        public async Task Convert_WithDirectConversion_ReturnsExpectedResult()
        {
            var converter = new CurrencyConverterService(this._apiRepository, this._logger);
            var Moneys = new List<Money> { new Money(100, "CNY") };
            var targetCurrency = new Currency("GBP");
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
                                { targetDate, new[] { rate } }
                            };

            A.CallTo(() => this._apiRepository.Get(targetDate, targetDate)).Returns(rates);

            var conversion = await converter.Convert(Moneys, targetCurrency, targetDate, this._ruleCtx);

            Assert.IsNotNull(conversion);
            Assert.AreEqual(conversion.Value.Value, 10);
            Assert.AreEqual(conversion.Value.Currency.Code, "GBP");
            A.CallTo(() => this._apiRepository.Get(targetDate, targetDate)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Convert_WithDirectConversionMultipleRates_ReturnsExpectedResult()
        {
            var converter = new CurrencyConverterService(this._apiRepository, this._logger);
            var monies = new List<Money> { new Money(100, "CNY"), new Money(20, "USD") };
            var targetCurrency = new Currency("GBP");
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
                                { targetDate, new[] { cnyRate, usdRate } }
                            };

            A.CallTo(() => this._apiRepository.Get(targetDate, targetDate)).Returns(rates);

            var conversion = await converter.Convert(monies, targetCurrency, targetDate, this._ruleCtx);

            Assert.IsNotNull(conversion);
            Assert.AreEqual(conversion.Value.Value, 20);
            Assert.AreEqual(conversion.Value.Currency.Code, "GBP");
            A.CallTo(() => this._apiRepository.Get(targetDate, targetDate)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Convert_WithDirectConversionMultipleRatesButOneIsMissing_ReturnsExpectedResult()
        {
            var converter = new CurrencyConverterService(this._apiRepository, this._logger);
            var Moneys = new List<Money> { new Money(100, "CNY"), new Money(20, "USD") };
            var targetCurrency = new Currency("GBP");
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
                                { targetDate, new[] { cnyRate } }
                            };

            A.CallTo(() => this._apiRepository.Get(targetDate, targetDate)).Returns(rates);

            var conversion = await converter.Convert(Moneys, targetCurrency, targetDate, this._ruleCtx);

            Assert.IsNotNull(conversion);
            Assert.AreEqual(conversion.Value.Value, 10);
            Assert.AreEqual(conversion.Value.Currency.Code, "GBP");
            A.CallTo(() => this._apiRepository.Get(targetDate, targetDate)).MustHaveHappenedOnceExactly();
        }

        [TestCase(1, 1, 100)]
        [TestCase(0.1, 1, 10)]
        [TestCase(0.1, 0.1, 100)]
        [TestCase(1, 0.5, 200)]
        public async Task Convert_WithIndirectConversionRateSetOneRates_ReturnsExpectedResult(
            decimal rate1,
            decimal rate2,
            decimal expected)
        {
            var converter = new CurrencyConverterService(this._apiRepository, this._logger);
            var Moneys = new List<Money> { new Money(100, "CNY") };
            var targetCurrency = new Currency("EUR");
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
                                { targetDate, new[] { cnyRate, eurRate } }
                            };

            A.CallTo(() => this._apiRepository.Get(targetDate, targetDate)).Returns(rates);

            var conversion = await converter.Convert(Moneys, targetCurrency, targetDate, this._ruleCtx);

            Assert.IsNotNull(conversion);
            Assert.AreEqual(conversion.Value.Value, expected);
            Assert.AreEqual(conversion.Value.Currency.Code, "EUR");
            A.CallTo(() => this._apiRepository.Get(targetDate, targetDate)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Convert_WithReciprocalConversion_ReturnsExpectedResult()
        {
            var converter = new CurrencyConverterService(this._apiRepository, this._logger);
            var Moneys = new List<Money> { new Money(100, "CNY") };
            var targetCurrency = new Currency("GBP");
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
                                { targetDate, new[] { cnyRate } }
                            };

            A.CallTo(() => this._apiRepository.Get(targetDate, targetDate)).Returns(rates);

            var conversion = await converter.Convert(Moneys, targetCurrency, targetDate, this._ruleCtx);

            Assert.IsNotNull(conversion);
            Assert.AreEqual(conversion.Value.Value, 10);
            Assert.AreEqual(conversion.Value.Currency.Code, "GBP");
            A.CallTo(() => this._apiRepository.Get(targetDate, targetDate)).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._apiRepository = A.Fake<IExchangeRateApiCachingDecorator>();
            this._logger = A.Fake<ILogger<CurrencyConverterService>>();
            this._currency = new Currency("USD");
            this._conversionTime = new DateTime(2017, 8, 31);
        }
    }
}