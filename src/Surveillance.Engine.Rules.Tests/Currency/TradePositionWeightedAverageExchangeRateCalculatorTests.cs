namespace Surveillance.Engine.Rules.Tests.Currency
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Money;
    using Domain.Core.Trading.Orders;

    using FakeItEasy;

    using Infrastructure.Network.HttpClient;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using PollyFacade.Policies.Interfaces;

    using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Currency;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Tests.Helpers;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
    using Surveillance.Reddeer.ApiClient.ExchangeRate;

    [TestFixture]
    public class TradePositionWeightedAverageExchangeRateCalculatorTests
    {
        private ILogger<TradePositionWeightedAverageExchangeRateService> _calculatorLogger;

        private IApiClientConfiguration _configuration;

        private Currency _currency;

        private IExchangeRatesService _exchangeRatesService;

        private ILogger<ExchangeRateApi> _logger;

        private ILogger<ExchangeRatesService> _loggerExchRate;

        private IPolicyFactory _policyFactory;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        [Test]
        public void Constructor_Null_Exchange_Rates_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new TradePositionWeightedAverageExchangeRateService(null, this._calculatorLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new TradePositionWeightedAverageExchangeRateService(this._exchangeRatesService, null));
        }

        [SetUp]
        public void Setup()
        {
            this._logger = A.Fake<ILogger<ExchangeRateApi>>();
            this._loggerExchRate = A.Fake<ILogger<ExchangeRatesService>>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._calculatorLogger = A.Fake<ILogger<TradePositionWeightedAverageExchangeRateService>>();
            this._exchangeRatesService = A.Fake<IExchangeRatesService>();
            this._currency = new Currency("GBX");
            this._policyFactory = A.Fake<IPolicyFactory>();

            this._configuration = A.Fake<IApiClientConfiguration>();
            this._configuration.ClientServiceUrl = "http://localhost:8080";
            this._configuration.SurveillanceUserApiAccessToken = "uwat";
        }

        [Test]
        public async Task WeightedExchangeRate_One_Order_With_One_Fill_Volume_()
        {
            A.CallTo(
                () => this._exchangeRatesService.GetRate(
                    A<Currency>.Ignored,
                    A<Currency>.Ignored,
                    A<DateTime>.Ignored,
                    this._ruleCtx)).Returns(
                new ExchangeRateDto
                    {
                        DateTime = DateTime.UtcNow,
                        FixedCurrency = "GBP",
                        Name = "abc",
                        Rate = 1,
                        VariableCurrency = "GBP"
                    });

            var calculator = new TradePositionWeightedAverageExchangeRateService(
                this._exchangeRatesService,
                this._calculatorLogger);

            var pos1 = new Order().Random();
            pos1.OrderFilledVolume = 1;
            pos1.OrderOrderedVolume = 1;
            var position = new TradePosition(new List<Order> { pos1 });

            var werResult = await calculator.WeightedExchangeRate(position, new Currency("GBP"), this._ruleCtx);

            Assert.AreEqual(werResult, 1);
        }

        [Test]
        public async Task WeightedExchangeRate_One_Order_With_Zero_Fill_Volume()
        {
            var calculator = new TradePositionWeightedAverageExchangeRateService(
                this._exchangeRatesService,
                this._calculatorLogger);

            var pos1 = new Order().Random();
            pos1.OrderFilledVolume = null;
            pos1.OrderOrderedVolume = null;
            var position = new TradePosition(new List<Order> { pos1 });

            var werResult = await calculator.WeightedExchangeRate(position, new Currency("GBP"), this._ruleCtx);

            Assert.AreEqual(werResult, 0);
        }

        [Test]
        [Explicit("Integration test")]
        public async Task WeightedExchangeRate_Returns_ExpectedResult()
        {
            var clientFactory = new HttpClientFactory(new NullLogger<HttpClientFactory>());
            var repo = new ExchangeRateApi(this._configuration, clientFactory, this._policyFactory, this._logger);
            var repoDecorator = new ExchangeRateApiCachingDecorator(repo);
            var exchangeRates = new ExchangeRatesService(repoDecorator, this._loggerExchRate);
            var calculator = new TradePositionWeightedAverageExchangeRateService(exchangeRates, this._calculatorLogger);

            var tradeOne = new Order().Random();
            var tradeTwo = new Order().Random();
            var tradeThree = new Order().Random();

            tradeOne.FilledDate = new DateTime(2017, 01, 01);
            tradeTwo.FilledDate = new DateTime(2017, 10, 25);
            tradeThree.FilledDate = new DateTime(2017, 10, 25);

            var position = new TradePosition(new List<Order> { tradeOne, tradeTwo, tradeThree });

            var usd = new Currency("usd");

            var wer = await calculator.WeightedExchangeRate(position, usd, this._ruleCtx);
        }

        [Test]
        public async Task WeightedExchangeRate_Two_Order_With_Two_Fill_Volume_()
        {
            A.CallTo(
                () => this._exchangeRatesService.GetRate(
                    A<Currency>.Ignored,
                    A<Currency>.Ignored,
                    DateTime.UtcNow.Date,
                    this._ruleCtx)).Returns(
                new ExchangeRateDto
                    {
                        DateTime = DateTime.UtcNow,
                        FixedCurrency = "GBP",
                        Name = "abc",
                        Rate = 1,
                        VariableCurrency = "GBP"
                    });

            A.CallTo(
                () => this._exchangeRatesService.GetRate(
                    A<Currency>.Ignored,
                    A<Currency>.Ignored,
                    DateTime.UtcNow.Date.AddDays(1),
                    this._ruleCtx)).Returns(
                new ExchangeRateDto
                    {
                        DateTime = DateTime.UtcNow,
                        FixedCurrency = "GBP",
                        Name = "abc",
                        Rate = 3,
                        VariableCurrency = "GBP"
                    });

            var calculator = new TradePositionWeightedAverageExchangeRateService(
                this._exchangeRatesService,
                this._calculatorLogger);

            var pos1 = new Order().Random();
            pos1.OrderFilledVolume = 1;
            pos1.OrderOrderedVolume = 1;

            var pos2 = new Order().Random();
            pos2.OrderFilledVolume = 1;
            pos2.OrderOrderedVolume = 1;
            pos2.AmendedDate = DateTime.UtcNow.Date.AddDays(1);
            pos2.BookedDate = DateTime.UtcNow.Date.AddDays(1);
            pos2.CancelledDate = DateTime.UtcNow.Date.AddDays(1);
            pos2.FilledDate = DateTime.UtcNow.Date.AddDays(1);
            pos2.PlacedDate = DateTime.UtcNow.Date.AddDays(1);
            pos2.RejectedDate = DateTime.UtcNow.Date.AddDays(1);

            var position = new TradePosition(new List<Order> { pos1, pos2 });

            var werResult = await calculator.WeightedExchangeRate(position, new Currency("GBP"), this._ruleCtx);

            Assert.AreEqual(werResult, 2);
        }

        [Test]
        public async Task WeightedExchangeRate_Two_Order_With_Unbalanced_Fill()
        {
            A.CallTo(
                () => this._exchangeRatesService.GetRate(
                    A<Currency>.Ignored,
                    A<Currency>.Ignored,
                    DateTime.UtcNow.Date,
                    this._ruleCtx)).Returns(
                new ExchangeRateDto
                    {
                        DateTime = DateTime.UtcNow,
                        FixedCurrency = "GBP",
                        Name = "abc",
                        Rate = 1,
                        VariableCurrency = "GBP"
                    });

            A.CallTo(
                () => this._exchangeRatesService.GetRate(
                    A<Currency>.Ignored,
                    A<Currency>.Ignored,
                    DateTime.UtcNow.Date.AddDays(1),
                    this._ruleCtx)).Returns(
                new ExchangeRateDto
                    {
                        DateTime = DateTime.UtcNow,
                        FixedCurrency = "GBP",
                        Name = "abc",
                        Rate = 4,
                        VariableCurrency = "GBP"
                    });

            var calculator = new TradePositionWeightedAverageExchangeRateService(
                this._exchangeRatesService,
                this._calculatorLogger);

            var pos1 = new Order().Random();
            pos1.OrderFilledVolume = 20;
            pos1.OrderOrderedVolume = 20;

            var pos2 = new Order().Random();
            pos2.OrderFilledVolume = 10;
            pos2.OrderOrderedVolume = 10;
            pos2.AmendedDate = DateTime.UtcNow.Date.AddDays(1);
            pos2.BookedDate = DateTime.UtcNow.Date.AddDays(1);
            pos2.CancelledDate = DateTime.UtcNow.Date.AddDays(1);
            pos2.FilledDate = DateTime.UtcNow.Date.AddDays(1);
            pos2.PlacedDate = DateTime.UtcNow.Date.AddDays(1);
            pos2.RejectedDate = DateTime.UtcNow.Date.AddDays(1);

            var position = new TradePosition(new List<Order> { pos1, pos2 });

            var werResult = await calculator.WeightedExchangeRate(position, new Currency("GBP"), this._ruleCtx);

            Assert.AreEqual(Math.Round(werResult, 2), 2);
        }

        [Test]
        public async Task WeightedExchangeRate_With_Null_Position_Returns_Zero()
        {
            var calculator = new TradePositionWeightedAverageExchangeRateService(
                this._exchangeRatesService,
                this._calculatorLogger);

            await calculator.WeightedExchangeRate(null, this._currency, this._ruleCtx);
        }
    }
}