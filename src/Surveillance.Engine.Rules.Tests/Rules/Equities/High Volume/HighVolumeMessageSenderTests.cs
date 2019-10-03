namespace Surveillance.Engine.Rules.Tests.Rules.Equities.High_Volume
{
    using System;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Trading.Orders;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora.Rules.Interfaces;
    using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;

    using TestHelpers;

    [TestFixture]
    public class HighVolumeMessageSenderTests
    {
        private ILogger<IHighVolumeMessageSender> logger;

        private IQueueCasePublisher queueCasePublisher;

        private IRuleBreachRepository repository;

        private IRuleBreachOrdersRepository ordersRepository;

        private IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper;

        private IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper;

        private Market market;

        [SetUp]
        public void Setup()
        {
            this.logger = A.Fake<ILogger<IHighVolumeMessageSender>>();
            this.queueCasePublisher = A.Fake<IQueueCasePublisher>();
            this.repository = A.Fake<IRuleBreachRepository>();
            this.ordersRepository = A.Fake<IRuleBreachOrdersRepository>();
            this.ruleBreachToRuleBreachOrdersMapper = A.Fake<IRuleBreachToRuleBreachOrdersMapper>();
            this.ruleBreachToRuleBreachMapper = A.Fake<IRuleBreachToRuleBreachMapper>();
            this.market = new Market("a", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
        }

        [Test]
        public async Task MessageSender_ConstructsExpectedString_FromDailyRuleBreach()
        {
            // arrange
            var messageSender = new HighVolumeMessageSender(
                this.logger,
                this.queueCasePublisher,
                this.repository,
                this.ordersRepository,
                this.ruleBreachToRuleBreachOrdersMapper,
                this.ruleBreachToRuleBreachMapper);

            var dailyBreach = new HighVolumeRuleBreach.BreachDetails(true, 0.4212m, 103918412, this.market);
            var ruleBreach = A.Fake<IHighVolumeRuleBreach>();
            A.CallTo(() => ruleBreach.DailyBreach).Returns(dailyBreach);
            A.CallTo(() => ruleBreach.TotalOrdersTradedInWindow).Returns(10241.0000m);

            var parameters = A.Fake<IHighVolumeRuleEquitiesParameters>();
            A.CallTo(() => parameters.HighVolumePercentageDaily).Returns(0.32123m);

            var order = new Order().Random();
            A.CallTo(() => ruleBreach.Security).Returns(order.Instrument);
            A.CallTo(() => ruleBreach.EquitiesParameters).Returns(parameters);
            
            // act
            var message = messageSender.BuildDescription(ruleBreach);
            
            // assert
            Assert.AreEqual("High Volume rule breach detected for random-security. Percentage of daily volume breach has occured. A daily volume limit of 33% was exceeded by trading 43% of daily volume at the venue (XLON) London Stock Exchange. 10241 volume was the allocated fill against a breach threshold volume of 103918412.", message);
        }

        [Test]
        public async Task MessageSender_ConstructsExpectedString_FromWindowRuleBreach()
        {
            // arrange
            var messageSender = new HighVolumeMessageSender(
                this.logger,
                this.queueCasePublisher,
                this.repository,
                this.ordersRepository,
                this.ruleBreachToRuleBreachOrdersMapper,
                this.ruleBreachToRuleBreachMapper);

            var venueVolumeBreach = new HighVolumeRuleBreach.BreachDetails(true, 0.4212m, 103918412, this.market);
            var ruleBreach = A.Fake<IHighVolumeRuleBreach>();
            A.CallTo(() => ruleBreach.WindowBreach).Returns(venueVolumeBreach);
            A.CallTo(() => ruleBreach.TotalOrdersTradedInWindow).Returns(10241.0000m);

            var parameters = A.Fake<IHighVolumeRuleEquitiesParameters>();
            A.CallTo(() => parameters.HighVolumePercentageWindow).Returns(0.32123m);
            A.CallTo(() => parameters.Windows).Returns(new TimeWindows("a", TimeSpan.FromHours(3)));

            var order = new Order().Random();
            A.CallTo(() => ruleBreach.Security).Returns(order.Instrument);
            A.CallTo(() => ruleBreach.EquitiesParameters).Returns(parameters);

            // act
            var message = messageSender.BuildDescription(ruleBreach);

            // assert
            Assert.AreEqual("High Volume rule breach detected for random-security. Percentage of window volume breach has occured. A window volume limit of 33% was exceeded by trading 43% of window volume within the window of 180 minutes at the venue (XLON) London Stock Exchange. 10241 volume was the allocated fill against a breach threshold volume of 103918412.", message);
        }

        [Test]
        public async Task MessageSender_ConstructsExpectedString_FromMarketCapRuleBreach()
        {
            // arrange
            var messageSender = new HighVolumeMessageSender(
                this.logger,
                this.queueCasePublisher,
                this.repository,
                this.ordersRepository,
                this.ruleBreachToRuleBreachOrdersMapper,
                this.ruleBreachToRuleBreachMapper);

            var marketCapBreach =
                new HighVolumeRuleBreach.BreachDetails(
                    true,
                    0.4212m,
                    new Money(10029, "GBX"),
                    new Money(99924.12m, "USD"),
                    this.market);

            var ruleBreach = A.Fake<IHighVolumeRuleBreach>();
            A.CallTo(() => ruleBreach.MarketCapBreach).Returns(marketCapBreach);
            A.CallTo(() => ruleBreach.TotalOrdersTradedInWindow).Returns(10241.000300m);

            var parameters = A.Fake<IHighVolumeRuleEquitiesParameters>();
            A.CallTo(() => parameters.HighVolumePercentageMarketCap).Returns(0.12123m);
            A.CallTo(() => parameters.Windows).Returns(new TimeWindows("a", TimeSpan.FromHours(3)));

            var order = new Order().Random();
            A.CallTo(() => ruleBreach.Security).Returns(order.Instrument);
            A.CallTo(() => ruleBreach.EquitiesParameters).Returns(parameters);

            // act
            var message = messageSender.BuildDescription(ruleBreach);

            // assert
            Assert.AreEqual("High Volume rule breach detected for random-security. Percentage of market capitalisation breach has occured. A limit of 13% was exceeded by trading 43% of market capitalisation.  (USD) 99924.12 was traded against a breach threshold value of (USD) 10029.", message);
        }
    }
}
