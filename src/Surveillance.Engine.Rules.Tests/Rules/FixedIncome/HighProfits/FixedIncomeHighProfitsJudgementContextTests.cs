namespace Surveillance.Engine.Rules.Tests.Rules.FixedIncome.HighProfits
{
    using System;

    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

    /// <summary>
    /// The fixed income high profits judgement context tests.
    /// </summary>
    [TestFixture]
    public class FixedIncomeHighProfitsJudgementContextTests
    {
        /// <summary>
        /// The fixed income high profit judgement.
        /// </summary>
        private IFixedIncomeHighProfitJudgement fixedIncomeHighProfitJudgement;

        /// <summary>
        /// The rule breach context.
        /// </summary>
        private IRuleBreachContext ruleBreachContext;

        /// <summary>
        /// The fixed income parameters.
        /// </summary>
        private IHighProfitsRuleFixedIncomeParameters fixedIncomeParameters;

        /// <summary>
        /// The profit breakdown.
        /// </summary>
        private IExchangeRateProfitBreakdown profitBreakdown;

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.fixedIncomeHighProfitJudgement = A.Fake<IFixedIncomeHighProfitJudgement>();
            this.ruleBreachContext = A.Fake<IRuleBreachContext>();
            this.fixedIncomeParameters = A.Fake<IHighProfitsRuleFixedIncomeParameters>();
            this.profitBreakdown = A.Fake<IExchangeRateProfitBreakdown>();
        }

        /// <summary>
        /// The constructor null judgement throws null.
        /// </summary>
        [Test]
        public void ConstructorNullJudgementThrowsNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FixedIncomeHighProfitJudgementContext(null, true));
        }

        /// <summary>
        /// The constructor properties set for judgement and rule violation.
        /// </summary>
        [Test]
        public void ConstructorPropertiesSetForJudgementAndRuleViolation()
        {
            var context = new FixedIncomeHighProfitJudgementContext(this.fixedIncomeHighProfitJudgement, true);

            Assert.IsTrue(context.RaiseRuleViolation);
            Assert.AreEqual(this.fixedIncomeHighProfitJudgement, context.Judgement);
        }

        /// <summary>
        /// The constructor sets properties for all.
        /// </summary>
        [Test]
        public void ConstructorSetsPropertiesForAll()
        {
            var context =
                new FixedIncomeHighProfitJudgementContext(
                    this.fixedIncomeHighProfitJudgement,
                    true,
                    this.ruleBreachContext,
                    this.fixedIncomeParameters,
                    null,
                    "currency-1",
                    100.2m,
                    true,
                    false,
                    this.profitBreakdown);

            Assert.IsTrue(context.RaiseRuleViolation);
            Assert.AreEqual(this.fixedIncomeHighProfitJudgement, context.Judgement);
            Assert.AreEqual("currency-1", context.AbsoluteProfitCurrency);
            Assert.AreEqual(null, context.AbsoluteProfits);
            Assert.AreEqual(this.fixedIncomeParameters, context.FixedIncomeParameters);
            Assert.AreEqual(this.profitBreakdown, context.ExchangeRateProfits);
            Assert.AreEqual(true, context.HasAbsoluteProfitBreach);
            Assert.AreEqual(false, context.HasRelativeProfitBreach);
            Assert.AreEqual(true, context.RaiseRuleViolation);
            Assert.AreEqual(100.2m, context.RelativeProfits);
            Assert.AreEqual(this.ruleBreachContext, context.RuleBreachContext);
        }
    }
}
