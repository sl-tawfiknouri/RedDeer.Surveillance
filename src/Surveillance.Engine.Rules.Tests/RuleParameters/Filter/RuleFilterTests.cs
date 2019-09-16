namespace Surveillance.Engine.Rules.Tests.RuleParameters.Filter
{
    using NUnit.Framework;

    using Surveillance.Engine.Rules.RuleParameters.Filter;

    /// <summary>
    /// The rule filter tests.
    /// </summary>
    [TestFixture]
    public class RuleFilterTests
    {
        /// <summary>
        /// The constructor sets ids non null.
        /// </summary>
        [Test]
        public void ConstructorSetsIdsNonNull()
        {
            var ruleFilter = new RuleFilter();

            Assert.IsNotNull(ruleFilter.Ids);
        }

        /// <summary>
        /// The none sets new rule filter with rule type none.
        /// </summary>
        [Test]
        public void NoneSetsNewRuleFilterWithRuleTypeNone()
        {
            var filter = RuleFilter.None();

            Assert.AreEqual(RuleFilterType.None, filter.Type);
            Assert.IsNotNull(filter.Ids);
        }
    }
}
