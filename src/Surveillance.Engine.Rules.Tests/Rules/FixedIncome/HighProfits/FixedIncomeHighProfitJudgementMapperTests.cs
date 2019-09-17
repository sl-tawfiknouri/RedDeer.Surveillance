namespace Surveillance.Engine.Rules.Tests.Rules.FixedIncome.HighProfits
{
    using System;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

    /// <summary>
    /// The fixed income high profit judgement mapper tests.
    /// </summary>
    [TestFixture]
    public class FixedIncomeHighProfitJudgementMapperTests
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<FixedIncomeHighProfitJudgementMapper> logger;

        /// <summary>
        /// The judgement context.
        /// </summary>
        private IFixedIncomeHighProfitJudgementContext judgementContext;

        /// <summary>
        /// The rule breach context.
        /// </summary>
        private IRuleBreachContext ruleBreachContext;

        /// <summary>
        /// The parameters.
        /// </summary>
        private IHighProfitsRuleFixedIncomeParameters parameters;

        private IExchangeRateProfitBreakdown profitBreakdown;

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.logger = A.Fake<ILogger<FixedIncomeHighProfitJudgementMapper>>();
            this.judgementContext = A.Fake<IFixedIncomeHighProfitJudgementContext>();
            this.ruleBreachContext = A.Fake<IRuleBreachContext>();
            this.parameters = A.Fake<IHighProfitsRuleFixedIncomeParameters>();
            this.profitBreakdown = A.Fake<IExchangeRateProfitBreakdown>();
        }

        /// <summary>
        /// The constructor null logger throws argument null exception.
        /// </summary>
        [Test]
        public void ConstructorNullLoggerThrowsArgumentNullException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FixedIncomeHighProfitJudgementMapper(null));
        }

        /// <summary>
        /// The map null context returns null.
        /// </summary>
        [Test]
        public void MapNullContextReturnsNull()
        {
            var mapper = this.BuildMapper();

            var result = mapper.Map(null);

            Assert.IsNull(result);
        }

        /// <summary>
        /// The map context builds expected rule breach.
        /// </summary>
        [Test]
        public void MapContextBuildsExpectedRelativeProfitBreachRuleBreach()
        {
            var mapper = this.BuildMapper();
            var fi = new FinancialInstrument { Name = "GILT 5 YEAR MATURITY" };

            A.CallTo(() => this.ruleBreachContext.Security).Returns(fi);
            A.CallTo(() => this.ruleBreachContext.UniverseDateTime).Returns(new DateTime(2019, 01, 01, 15, 03, 0));

            A.CallTo(() => this.parameters.HighProfitPercentageThreshold).Returns(0.1m);

            A.CallTo(() => this.judgementContext.FixedIncomeParameters).Returns(this.parameters);
            A.CallTo(() => this.judgementContext.HasRelativeProfitBreach).Returns(true);
            A.CallTo(() => this.judgementContext.RelativeProfits).Returns(0.124m);
            A.CallTo(() => this.judgementContext.RaiseRuleViolation).Returns(true);
            A.CallTo(() => this.judgementContext.RuleBreachContext).Returns(this.ruleBreachContext);

            var result = mapper.Map(this.judgementContext);

            Assert.AreEqual("Automated Fixed Income High Profit Rule Breach Detected", result.CaseTitle);
            Assert.AreEqual("High profit rule breach detected for GILT 5 YEAR MATURITY. There was a high profit ratio of 12.40% which exceeded the configured high profit ratio percentage threshold of 10.0%.", result.Description);
        }

        /// <summary>
        /// The map context builds expected rule breach.
        /// </summary>
        [Test]
        public void MapContextBuildsExpectedAbsoluteProfitBreachRuleBreach()
        {
            var mapper = this.BuildMapper();
            var fi = new FinancialInstrument { Name = "GILT 5 YEAR MATURITY" };

            A.CallTo(() => this.ruleBreachContext.Security).Returns(fi);
            A.CallTo(() => this.ruleBreachContext.UniverseDateTime).Returns(new DateTime(2019, 01, 01, 15, 03, 0));

            A.CallTo(() => this.parameters.HighProfitAbsoluteThreshold).Returns(12000m);
            A.CallTo(() => this.parameters.HighProfitCurrencyConversionTargetCurrency).Returns("GBP");

            A.CallTo(() => this.judgementContext.FixedIncomeParameters).Returns(this.parameters);
            A.CallTo(() => this.judgementContext.HasAbsoluteProfitBreach).Returns(true);
            A.CallTo(() => this.judgementContext.AbsoluteProfits).Returns(new Money(12341m, "GBP"));
            A.CallTo(() => this.judgementContext.RaiseRuleViolation).Returns(true);
            A.CallTo(() => this.judgementContext.RuleBreachContext).Returns(this.ruleBreachContext);

            var result = mapper.Map(this.judgementContext);

            Assert.AreEqual("Automated Fixed Income High Profit Rule Breach Detected", result.CaseTitle);
            Assert.AreEqual("High profit rule breach detected for GILT 5 YEAR MATURITY. There was a high profit of 12341 (GBP) which exceeded the configured profit limit of 12000(GBP).", result.Description);
        }

        /// <summary>
        /// The map context builds expected rule breach.
        /// </summary>
        [Test]
        public void MapContextBuildsExpectedAbsoluteProfitExchangeWeightedBreachRuleBreach()
        {
            var mapper = this.BuildMapper();
            var fi = new FinancialInstrument { Name = "GILT 5 YEAR MATURITY" };

            A.CallTo(() => this.ruleBreachContext.Security).Returns(fi);
            A.CallTo(() => this.ruleBreachContext.UniverseDateTime).Returns(new DateTime(2019, 01, 01, 15, 03, 0));

            A.CallTo(() => this.parameters.UseCurrencyConversions).Returns(true);
            A.CallTo(() => this.parameters.HighProfitAbsoluteThreshold).Returns(12000m);
            A.CallTo(() => this.parameters.HighProfitCurrencyConversionTargetCurrency).Returns("GBP");

            A.CallTo(() => this.judgementContext.FixedIncomeParameters).Returns(this.parameters);
            A.CallTo(() => this.judgementContext.HasAbsoluteProfitBreach).Returns(true);
            A.CallTo(() => this.judgementContext.AbsoluteProfits).Returns(new Money(12341m, "GBP"));
            A.CallTo(() => this.judgementContext.RaiseRuleViolation).Returns(true);
            A.CallTo(() => this.judgementContext.RuleBreachContext).Returns(this.ruleBreachContext);
            A.CallTo(() => this.judgementContext.ExchangeRateProfits).Returns(this.profitBreakdown);

            A.CallTo(() => this.profitBreakdown.FixedCurrency).Returns(new Currency("GBP"));
            A.CallTo(() => this.profitBreakdown.VariableCurrency).Returns(new Currency("USD"));
            A.CallTo(() => this.profitBreakdown.AbsoluteAmountDueToWer()).Returns(9001);
            A.CallTo(() => this.profitBreakdown.PositionCostWer).Returns(0.12m);
            A.CallTo(() => this.profitBreakdown.PositionRevenueWer).Returns(0.32m);
            A.CallTo(() => this.profitBreakdown.RelativePercentageDueToWer()).Returns(0.99m);

            var result = mapper.Map(this.judgementContext);

            Assert.AreEqual("Automated Fixed Income High Profit Rule Breach Detected", result.CaseTitle);
            Assert.AreEqual("High profit rule breach detected for GILT 5 YEAR MATURITY. There was a high profit of 12341 (GBP) which exceeded the configured profit limit of 12000(GBP). The position was acquired with a currency conversion between (GBP/USD) rate at a weighted exchange rate of 0.12 and sold at a weighted exchange rate of 0.32. The impact on profits from exchange rate movements was 99.00% and the absolute amount of profits due to exchange rates is () 9001.", result.Description);
        }

        /// <summary>
        /// The build mapper.
        /// </summary>
        /// <returns>
        /// The <see cref="FixedIncomeHighProfitJudgementMapper"/>.
        /// </returns>
        private FixedIncomeHighProfitJudgementMapper BuildMapper()
        {
            return new FixedIncomeHighProfitJudgementMapper(this.logger);
        }
    }
}
