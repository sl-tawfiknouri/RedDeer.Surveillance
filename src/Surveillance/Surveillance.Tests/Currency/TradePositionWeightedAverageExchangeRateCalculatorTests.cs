using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Currency;
using Surveillance.Currency.Interfaces;
using Surveillance.DataLayer.Api.ExchangeRate;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Tests.Helpers;
using Surveillance.Trades;

namespace Surveillance.Tests.Currency
{
    [TestFixture]
    public class TradePositionWeightedAverageExchangeRateCalculatorTests
    {
        private ILogger<ExchangeRateApiRepository> _logger;
        private ILogger<ExchangeRates> _loggerExchRate;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IDataLayerConfiguration _configuration;
        private ILogger<TradePositionWeightedAverageExchangeRateCalculator> _calculatorLogger;
        private IExchangeRates _exchangeRates;
        private DomainV2.Financial.Currency _currency;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<ExchangeRateApiRepository>>();
            _loggerExchRate = A.Fake<ILogger<ExchangeRates>>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _calculatorLogger = A.Fake<ILogger<TradePositionWeightedAverageExchangeRateCalculator>>();
            _exchangeRates = A.Fake<IExchangeRates>();
            _currency = new DomainV2.Financial.Currency("GBX");

            _configuration = A.Fake<IDataLayerConfiguration>();
            _configuration.ClientServiceUrl = "http://localhost:8080";
            _configuration.SurveillanceUserApiAccessToken = "uwat";
        }

        [Test]
        public void Constructor_Null_Exchange_Rates_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new TradePositionWeightedAverageExchangeRateCalculator(null, _calculatorLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new TradePositionWeightedAverageExchangeRateCalculator(_exchangeRates, null));
        }

        [Test]
        public async Task WeightedExchangeRate_With_Null_Position_Returns_Zero()
        {
            var calculator = new TradePositionWeightedAverageExchangeRateCalculator(_exchangeRates, _calculatorLogger);

             await calculator.WeightedExchangeRate(null, _currency, _ruleCtx);
        }



        [Test]
        [Explicit("Integration test")]
        public async Task WeightedExchangeRate_Returns_ExpectedResult()
        {
            var repo = new ExchangeRateApiRepository(_configuration, _logger);
            var repoDecorator = new ExchangeRateApiCachingDecoratorRepository(repo);
            var exchangeRates = new ExchangeRates(repoDecorator, _loggerExchRate);
            var calculator = new TradePositionWeightedAverageExchangeRateCalculator(exchangeRates, _calculatorLogger);

            var tradeOne = (new Order()).Random();
            var tradeTwo = (new Order()).Random();
            var tradeThree = (new Order()).Random();

            tradeOne.FilledDate = new DateTime(2017, 01, 01);
            tradeTwo.FilledDate = new DateTime(2017, 10, 25);
            tradeThree.FilledDate = new DateTime(2017, 10, 25);

            var position = new TradePosition(new List<Order>
            {
                tradeOne,
                tradeTwo,
                tradeThree
            });

            var usd = new DomainV2.Financial.Currency("usd");

            var wer = await calculator.WeightedExchangeRate(
                position,
                usd,
                _ruleCtx);
        }
    }
}