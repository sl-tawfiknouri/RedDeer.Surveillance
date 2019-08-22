namespace Surveillance.Engine.Rules.Tests.Rules.Equities.High_Volume
{
    using System.Threading.Tasks;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Trading.Orders;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using TestHelpers;

    using Surveillance.DataLayer.Aurora.Rules.Interfaces;
    using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;

    [TestFixture]
    public class HighVolumeMessageSenderTests
    {
        private ILogger<IHighVolumeMessageSender> logger;

        private IQueueCasePublisher queueCasePublisher;

        private IRuleBreachRepository repository;

        private IRuleBreachOrdersRepository ordersRepository;

        private IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper;

        private IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper;

        [SetUp]
        public void Setup()
        {
            this.logger = A.Fake<ILogger<IHighVolumeMessageSender>>();
            this.queueCasePublisher = A.Fake<IQueueCasePublisher>();
            this.repository = A.Fake<IRuleBreachRepository>();
            this.ordersRepository = A.Fake<IRuleBreachOrdersRepository>();
            this.ruleBreachToRuleBreachOrdersMapper = A.Fake<IRuleBreachToRuleBreachOrdersMapper>();
            this.ruleBreachToRuleBreachMapper = A.Fake<IRuleBreachToRuleBreachMapper>();
        }

        [Test]
        public async Task MessageSender_ConstructsExpectedString_FromRuleBreach()
        {
            // arrange
            var messageSender = new HighVolumeMessageSender(
                this.logger,
                this.queueCasePublisher,
                this.repository,
                this.ordersRepository,
                this.ruleBreachToRuleBreachOrdersMapper,
                this.ruleBreachToRuleBreachMapper);

            var dailyBreach = new HighVolumeRuleBreach.BreachDetails(true, 0.4212m, 103918412);
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
            Assert.AreEqual("High Volume rule breach detected for random-security. Percentage of daily volume breach has occured. A daily volume limit of 33% was exceeded by trading 43% of daily volume. 10241.0000 volume was ordered against a breach threshold volume of 103918412.", message);
        }
    }
}
