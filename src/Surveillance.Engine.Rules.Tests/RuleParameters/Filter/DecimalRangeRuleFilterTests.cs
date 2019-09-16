namespace Surveillance.Engine.Rules.Tests.RuleParameters.Filter
{
    using NUnit.Framework;

    using Surveillance.Engine.Rules.RuleParameters.Filter;

    /// <summary>
    /// The decimal range rule filter tests.
    /// </summary>
    [TestFixture]
    public class DecimalRangeRuleFilterTests
    {
        /// <summary>
        /// The none returns new range filter with none filter type.
        /// </summary>
        [Test]
        public void NoneReturnsNewRangeFilterWithNoneFilterType()
        {
            var result = DecimalRangeRuleFilter.None();

            Assert.AreEqual(RuleFilterType.None, result.Type);
            Assert.AreEqual(null, result.Max);
            Assert.AreEqual(null, result.Min);
        }
    }
}
