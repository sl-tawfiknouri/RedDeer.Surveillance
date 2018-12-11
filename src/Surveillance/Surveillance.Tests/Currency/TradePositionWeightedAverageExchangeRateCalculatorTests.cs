using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Currency;
using Surveillance.DataLayer.Api.ExchangeRate;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
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

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<ExchangeRateApiRepository>>();
            _loggerExchRate = A.Fake<ILogger<ExchangeRates>>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();


            _configuration = A.Fake<IDataLayerConfiguration>();
            _configuration.ClientServiceUrl = "http://localhost:8080";
            _configuration.SurveillanceUserApiAccessToken = "uwat";
        }

        [Test]
        [Explicit("Integration test")]
        public async Task WeightedExchangeRate_Returns_ExpectedResult()
        {
            var repo = new ExchangeRateApiRepository(_configuration, _logger);
            var repoDecorator = new ExchangeRateApiCachingDecoratorRepository(repo);
            var exchangeRates = new ExchangeRates(repoDecorator, _loggerExchRate);
            var calculator = new TradePositionWeightedAverageExchangeRateCalculator(exchangeRates);

            Order tradeOne = (new Order()).Random();
            Order tradeTwo = (new Order()).Random();
            Order tradeThree = (new Order()).Random();

            tradeOne.OrderFilledDate = new DateTime(2017, 01, 01);
            tradeTwo.OrderFilledDate = new DateTime(2017, 10, 25);
            tradeThree.OrderFilledDate = new DateTime(2017, 10, 25);

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