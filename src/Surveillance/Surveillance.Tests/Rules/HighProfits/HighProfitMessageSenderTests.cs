using System.Collections.Generic;
using Domain.Equity;
using Domain.Trades.Orders;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Mappers;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.HighProfits;
using Surveillance.Rules.HighProfits.Calculators;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;

namespace Surveillance.Tests.Rules.HighProfits
{
    [TestFixture]
    public class HighProfitMessageSenderTests
    {
        private ILogger<HighProfitMessageSender> _logger;
        private ICaseMessageSender _messageSender;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IHighProfitsRuleParameters _parameters;
        private Security _security;

        [SetUp]
        public void Setup()
        {
            _messageSender = A.Fake<ICaseMessageSender>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _parameters = A.Fake<IHighProfitsRuleParameters>();
            A.CallTo(() => _parameters.UseCurrencyConversions).Returns(true);

            _logger = A.Fake<ILogger<HighProfitMessageSender>>();
            _security = new Security(new SecurityIdentifiers("id", "id", "id", "id", "id", "id", "id", "id", "id", "id"), "security", "cfi", "issuer-identifier");
        }

        [Test]
        [Explicit]
        public void DoesSendExchangeRateMessage_AsExpected()
        {
            var messageSender = new HighProfitMessageSender(
                new TradeOrderDataItemDtoMapper(),
                _logger,
                _messageSender);

            var exchangeRateProfitBreakdown =
                new ExchangeRateProfitBreakdown(
                    new TradePosition(new List<TradeOrderFrame>()),
                    new TradePosition(new List<TradeOrderFrame>()),
                    10m,
                    1m,
                    "USD",
                    "GBP");

            var breach =
                new HighProfitRuleBreach(
                    _parameters,
                    10m,
                    "GBP",
                    0.3m,
                    _security,
                    false,
                    false,
                    new TradePosition(new List<TradeOrderFrame>()),
                    false,
                    exchangeRateProfitBreakdown);

            messageSender.Send(breach, _ruleCtx);
        }
    }
}
