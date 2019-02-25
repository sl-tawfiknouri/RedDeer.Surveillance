using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Financial;
using Domain.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.HighProfits;
using Surveillance.Engine.Rules.Rules.HighProfits.Calculators;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades;

namespace Surveillance.Engine.Rules.Tests.Rules.HighProfits
{
    [TestFixture]
    public class HighProfitMessageSenderTests
    {
        private ILogger<HighProfitMessageSender> _logger;
        private IQueueCasePublisher _publisher;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private ISystemProcessOperationContext _opCtx;
        private IHighProfitsRuleParameters _parameters;
        private IRuleBreachRepository _ruleBreachRepository;
        private IRuleBreachOrdersRepository _ruleBreachOrdersRepository;
        private IRuleBreachToRuleBreachOrdersMapper _ruleBreachToRuleBreachOrdersMapper;
        private IRuleBreachToRuleBreachMapper _ruleBreachToRuleBreachMapper;
        private IFactorValue _factorValue;
        private FinancialInstrument _security;

        [SetUp]
        public void Setup()
        {
            _factorValue = A.Fake<IFactorValue>();
            _publisher = A.Fake<IQueueCasePublisher>();
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
                _publisher,
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
                    new Domain.Financial.Currency("USD"),
                    new Domain.Financial.Currency("GBP"));

            var breach =
                new HighProfitRuleBreach(
                    _factorValue,
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
