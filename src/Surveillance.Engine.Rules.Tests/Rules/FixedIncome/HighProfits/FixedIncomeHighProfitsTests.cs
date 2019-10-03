namespace Surveillance.Engine.Rules.Tests.Rules.FixedIncome.HighProfits
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    /// <summary>
    /// The fixed income high profits tests.
    /// </summary>
    [TestFixture]
    public class FixedIncomeHighProfitsTests
    {
        /// <summary>
        /// The stream rule.
        /// </summary>
        private IFixedIncomeHighProfitsStreamRule streamRule;

        /// <summary>
        /// The market closure rule.
        /// </summary>
        private IFixedIncomeHighProfitsMarketClosureRule marketClosureRule;

        /// <summary>
        /// The _parameters.
        /// </summary>
        private IHighProfitsRuleFixedIncomeParameters parameters;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<FixedIncomeHighProfitsRule> logger;

        /// <summary>
        /// The universe event.
        /// </summary>
        private IUniverseEvent universeEvent;

        /// <summary>
        /// The constructor logger null throws exception.
        /// </summary>
        [Test]
        public void ConstructorLoggerNullThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new FixedIncomeHighProfitsRule(
                    this.parameters,
                    this.streamRule,
                    this.marketClosureRule,
                    null));
        }

        /// <summary>
        /// The organization factor value default to none.
        /// </summary>
        [Test]
        public void OrganisationFactorValueDefaultToNone()
        {
            var rule = this.BuildRule();
            
            Assert.AreEqual("V1.0", rule.Version);
            Assert.AreEqual(FactorValue.None.Value, rule.OrganisationFactorValue.Value);
            Assert.AreEqual(Domain.Surveillance.Scheduling.Rules.FixedIncomeHighProfits, rule.Rule);
        }

        /// <summary>
        /// The clone with factor value yields new clone with sub rules cloned.
        /// </summary>
        [Test]
        public void CloneWithFactorValueYieldsNewCloneWithSubRulesCloned()
        {
            var factorValue = new FactorValue(ClientOrganisationalFactors.Fund, "fund-a");

            A.CallTo(() => this.streamRule.Clone(factorValue)).Returns(this.streamRule);
            A.CallTo(() => this.marketClosureRule.Clone(factorValue)).Returns(this.marketClosureRule);

            var rule = this.BuildRule();

            var clonedRule = rule.Clone(factorValue);

            Assert.AreNotEqual(clonedRule, rule);
            Assert.AreEqual(factorValue, clonedRule.OrganisationFactorValue);
            A.CallTo(() => this.streamRule.Clone(factorValue)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.marketClosureRule.Clone(factorValue)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The clone with factor value yields new clone with sub rules cloned.
        /// </summary>
        [Test]
        public void CloneWithoutFactorValueYieldsNewCloneWithSubRulesCloned()
        {
            A.CallTo(() => this.streamRule.Clone()).Returns(this.streamRule);
            A.CallTo(() => this.marketClosureRule.Clone()).Returns(this.marketClosureRule);

            var rule = this.BuildRule();

            var clonedRule = rule.Clone() as FixedIncomeHighProfitsRule;
            
            Assert.AreNotEqual(clonedRule, rule);
            Assert.IsNotNull(clonedRule);
            Assert.AreEqual(rule.OrganisationFactorValue.Value, clonedRule.OrganisationFactorValue.Value);
            A.CallTo(() => this.streamRule.Clone()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.marketClosureRule.Clone()).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The on completed calls sub rule completed.
        /// </summary>
        [Test]
        public void OnCompletedCallsSubRuleCompleted()
        {
            var rule = this.BuildRule();

            rule.OnCompleted();

            A.CallTo(() => this.streamRule.OnCompleted()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.marketClosureRule.OnCompleted()).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The on error passes through error to sub rules.
        /// </summary>
        [Test]
        public void OnErrorPassesThroughErrorToSubRules()
        {
            var rule = this.BuildRule();
            var exception = new ArgumentNullException();

            rule.OnError(exception);

            A.CallTo(() => this.streamRule.OnError(exception)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.marketClosureRule.OnError(exception)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The on next does not pass through event to sub rules when no parameters set.
        /// </summary>
        [Test]
        public void OnNextDoesNotPassThroughEventToSubRulesWhenNoParametersSet()
        {
            var rule = this.BuildRule();

            rule.OnNext(this.universeEvent);

            A.CallTo(() => this.streamRule.OnNext(this.universeEvent)).MustNotHaveHappened();
            A.CallTo(() => this.marketClosureRule.OnNext(this.universeEvent)).MustNotHaveHappened();
        }

        /// <summary>
        /// The on next does pass through event to sub rules when parameters set for window.
        /// </summary>
        [Test]
        public void OnNextDoesPassThroughEventToSubRulesWhenParametersSetForWindow()
        {
            A.CallTo(() => this.parameters.PerformHighProfitWindowAnalysis).Returns(true);
            var rule = this.BuildRule();

            rule.OnNext(this.universeEvent);

            A.CallTo(() => this.streamRule.OnNext(this.universeEvent)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.marketClosureRule.OnNext(this.universeEvent)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The on next does pass through event to sub rules when parameters set for window and daily.
        /// </summary>
        [Test]
        public void OnNextDoesPassThroughEventToSubRulesWhenParametersSetForWindowAndDaily()
        {
            A.CallTo(() => this.parameters.PerformHighProfitWindowAnalysis).Returns(true);
            A.CallTo(() => this.parameters.PerformHighProfitDailyAnalysis).Returns(true);
            var rule = this.BuildRule();

            rule.OnNext(this.universeEvent);

            A.CallTo(() => this.streamRule.OnNext(this.universeEvent)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.marketClosureRule.OnNext(this.universeEvent)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The on next does pass through event to sub rules when parameters set for daily only.
        /// </summary>
        [Test]
        public void OnNextDoesPassThroughEventToSubRulesWhenParametersSetForDailyOnly()
        {
            A.CallTo(() => this.parameters.PerformHighProfitDailyAnalysis).Returns(true);
            var rule = this.BuildRule();

            rule.OnNext(this.universeEvent);

            A.CallTo(() => this.streamRule.OnNext(this.universeEvent)).MustNotHaveHappened();
            A.CallTo(() => this.marketClosureRule.OnNext(this.universeEvent)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The setup test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.streamRule = A.Fake<IFixedIncomeHighProfitsStreamRule>();
            this.marketClosureRule = A.Fake<IFixedIncomeHighProfitsMarketClosureRule>();
            this.parameters = A.Fake<IHighProfitsRuleFixedIncomeParameters>();
            this.logger = A.Fake<ILogger<FixedIncomeHighProfitsRule>>();
            this.universeEvent = A.Fake<IUniverseEvent>();
        }

        /// <summary>
        /// The build rule.
        /// </summary>
        /// <returns>
        /// The <see cref="FixedIncomeHighProfitsRule"/>.
        /// </returns>
        private FixedIncomeHighProfitsRule BuildRule()
        {
            return new FixedIncomeHighProfitsRule(this.parameters, this.streamRule, this.marketClosureRule, this.logger);
        }
    }
}