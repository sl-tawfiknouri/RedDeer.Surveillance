namespace Surveillance.Engine.Rules.Tests.RuleParameters.Extensions
{
    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.RuleParameters.Extensions;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    /// <summary>
    /// The market cap filterable extensions tests.
    /// </summary>
    [TestFixture]
    public class MarketCapFilterableExtensionsTests
    {
        /// <summary>
        /// The market cap filterable.
        /// </summary>
        private IMarketCapFilterable marketCapFilterable;

        /// <summary>
        /// The setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.marketCapFilterable = A.Fake<IMarketCapFilterable>();
        }

        /// <summary>
        /// The has market cap filters returns true for include only.
        /// </summary>
        /// <param name="filterType">
        /// The filter type.
        /// </param>
        /// <param name="expectation">
        /// The expectation.
        /// </param>
        [TestCase(RuleFilterType.None, false)]
        [TestCase(RuleFilterType.Exclude, false)]
        [TestCase(RuleFilterType.Include, true)]
        public void HasMarketCapFiltersReturnsTrueForIncludeOnly(RuleFilterType filterType, bool expectation)
        {
            A.CallTo(() => this.marketCapFilterable.MarketCapFilter).Returns(new DecimalRangeRuleFilter { Type = filterType });

            var result = MarketCapFilterableExtensions.HasMarketCapFilters(this.marketCapFilterable);

            Assert.AreEqual(result, expectation);
        }
    }
}
