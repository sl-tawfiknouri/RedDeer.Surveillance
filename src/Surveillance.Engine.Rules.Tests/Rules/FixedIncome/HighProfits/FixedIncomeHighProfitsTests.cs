namespace Surveillance.Engine.Rules.Tests.Rules.FixedIncome.HighProfits
{
    using System;

    using FakeItEasy;
    
    using NUnit.Framework;

    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;

    [TestFixture]
    public class FixedIncomeHighProfitsTests
    {
        private IFixedIncomeHighProfitsStreamRule streamRule;

        private IFixedIncomeHighProfitsMarketClosureRule marketClosureRule;

        private IHighProfitsRuleFixedIncomeParameters _parameters;

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new FixedIncomeHighProfitsRule(
                    this._parameters,
                    this.streamRule,
                    this.marketClosureRule,
                    null));
        }

        [SetUp]
        public void Setup()
        {
            this.streamRule = A.Fake<IFixedIncomeHighProfitsStreamRule>();
            this.marketClosureRule = A.Fake<IFixedIncomeHighProfitsMarketClosureRule>();
            this._parameters = A.Fake<IHighProfitsRuleFixedIncomeParameters>();
        }
    }
}