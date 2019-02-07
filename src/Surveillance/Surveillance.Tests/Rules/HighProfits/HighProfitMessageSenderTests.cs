﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Financial;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.Mappers.RuleBreach.Interfaces;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.HighProfits;
using Surveillance.Rules.HighProfits.Calculators;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Trades;

namespace Surveillance.Tests.Rules.HighProfits
{
    [TestFixture]
    public class HighProfitMessageSenderTests
    {
        private ILogger<HighProfitMessageSender> _logger;
        private ICaseMessageSender _messageSender;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private ISystemProcessOperationContext _opCtx;
        private IHighProfitsRuleParameters _parameters;
        private IRuleBreachRepository _ruleBreachRepository;
        private IRuleBreachOrdersRepository _ruleBreachOrdersRepository;
        private IRuleBreachToRuleBreachOrdersMapper _ruleBreachToRuleBreachOrdersMapper;
        private IRuleBreachToRuleBreachMapper _ruleBreachToRuleBreachMapper;
        private FinancialInstrument _security;

        [SetUp]
        public void Setup()
        {
            _messageSender = A.Fake<ICaseMessageSender>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _opCtx = A.Fake<ISystemProcessOperationContext>();
            _parameters = A.Fake<IHighProfitsRuleParameters>();
            A.CallTo(() => _parameters.UseCurrencyConversions).Returns(true);

            _ruleBreachRepository = A.Fake<IRuleBreachRepository>();
            _ruleBreachOrdersRepository = A.Fake<IRuleBreachOrdersRepository>();
            _ruleBreachToRuleBreachOrdersMapper = A.Fake<IRuleBreachToRuleBreachOrdersMapper>();
            _ruleBreachToRuleBreachMapper = A.Fake<IRuleBreachToRuleBreachMapper>();
            _logger = A.Fake<ILogger<HighProfitMessageSender>>();
            _security =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    new InstrumentIdentifiers("id", "id", "id", "id", "id", "id", "id", "id", "id", "id", "id"),
                    "security",
                    "cfi", 
                    "USD",
                    "issuer-identifier");
        }

        [Test]
        [Explicit]
        public async Task DoesSendExchangeRateMessage_AsExpected()
        {
            var messageSender = new HighProfitMessageSender(
                _logger,
                _messageSender,
                _ruleBreachRepository,
                _ruleBreachOrdersRepository,
                _ruleBreachToRuleBreachOrdersMapper,
                _ruleBreachToRuleBreachMapper);

            var exchangeRateProfitBreakdown =
                new ExchangeRateProfitBreakdown(
                    new TradePosition(new List<Order>()),
                    new TradePosition(new List<Order>()),
                    10m,
                    1m,
                    new DomainV2.Financial.Currency("USD"),
                    new DomainV2.Financial.Currency("GBP"));

            var breach =
                new HighProfitRuleBreach(
                    _opCtx,
                    "correlation-id",
                    _parameters,
                    10m,
                    "GBP",
                    0.3m,
                    _security,
                    false,
                    false,
                    new TradePosition(new List<Order>()),
                    false,
                    exchangeRateProfitBreakdown);

            await messageSender.Send(breach);
        }
    }
}
