namespace Surveillance.Engine.Rules.Tests.RuleParameters.Extensions
{
    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.RuleParameters.Extensions;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    /// <summary>
    /// The filterable rule extensions tests.
    /// </summary>
    [TestFixture]
    public class FilterableRuleExtensionsTests
    {
        /// <summary>
        /// The filterable rule.
        /// </summary>
        private IFilterableRule filterableRule;

        /// <summary>
        /// The setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.filterableRule = A.Fake<IFilterableRule>();
        }

        /// <summary>
        /// The has internal filters none set returns false.
        /// </summary>
        [Test]
        public void HasInternalFiltersNoneSetReturnsFalse()
        {
            var result = FilterableRuleExtensions.HasInternalFilters(this.filterableRule);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// The has internal filters set accounts returns true.
        /// </summary>
        /// <param name="filterType">
        /// The filter type.
        /// </param>
        /// <param name="expectation">
        /// The expectation.
        /// </param>
        [TestCase(RuleFilterType.None, false)]
        [TestCase(RuleFilterType.Include, true)]
        [TestCase(RuleFilterType.Exclude, true)]
        public void HasInternalFiltersSetAccountsReturnsTrue(RuleFilterType filterType, bool expectation)
        {
            A.CallTo(() => this.filterableRule.Accounts).Returns(new RuleFilter { Type = filterType });
            var result = FilterableRuleExtensions.HasInternalFilters(this.filterableRule);

            Assert.AreEqual(result, expectation);
        }

        /// <summary>
        /// The has internal filters set traders returns true.
        /// </summary>
        /// <param name="filterType">
        /// The filter type.
        /// </param>
        /// <param name="expectation">
        /// The expectation.
        /// </param>
        [TestCase(RuleFilterType.None, false)]
        [TestCase(RuleFilterType.Include, true)]
        [TestCase(RuleFilterType.Exclude, true)]
        public void HasInternalFiltersSetTradersReturnsTrue(RuleFilterType filterType, bool expectation)
        {
            A.CallTo(() => this.filterableRule.Traders).Returns(new RuleFilter { Type = filterType });
            var result = FilterableRuleExtensions.HasInternalFilters(this.filterableRule);

            Assert.AreEqual(result, expectation);
        }

        /// <summary>
        /// The has internal filters set markets returns true.
        /// </summary>
        /// <param name="filterType">
        /// The filter type.
        /// </param>
        /// <param name="expectation">
        /// The expectation.
        /// </param>
        [TestCase(RuleFilterType.None, false)]
        [TestCase(RuleFilterType.Include, true)]
        [TestCase(RuleFilterType.Exclude, true)]
        public void HasInternalFiltersSetMarketsReturnsTrue(RuleFilterType filterType, bool expectation)
        {
            A.CallTo(() => this.filterableRule.Markets).Returns(new RuleFilter { Type = filterType });
            var result = FilterableRuleExtensions.HasInternalFilters(this.filterableRule);

            Assert.AreEqual(result, expectation);
        }

        /// <summary>
        /// The has internal filters set funds returns true.
        /// </summary>
        /// <param name="filterType">
        /// The filter type.
        /// </param>
        /// <param name="expectation">
        /// The expectation.
        /// </param>
        [TestCase(RuleFilterType.None, false)]
        [TestCase(RuleFilterType.Include, true)]
        [TestCase(RuleFilterType.Exclude, true)]
        public void HasInternalFiltersSetFundsReturnsTrue(RuleFilterType filterType, bool expectation)
        {
            A.CallTo(() => this.filterableRule.Funds).Returns(new RuleFilter { Type = filterType });
            var result = FilterableRuleExtensions.HasInternalFilters(this.filterableRule);

            Assert.AreEqual(result, expectation);
        }

        /// <summary>
        /// The has internal filters set strategies returns true.
        /// </summary>
        /// <param name="filterType">
        /// The filter type.
        /// </param>
        /// <param name="expectation">
        /// The expectation.
        /// </param>
        [TestCase(RuleFilterType.None, false)]
        [TestCase(RuleFilterType.Include, true)]
        [TestCase(RuleFilterType.Exclude, true)]
        public void HasInternalFiltersSetStrategiesReturnsTrue(RuleFilterType filterType, bool expectation)
        {
            A.CallTo(() => this.filterableRule.Strategies).Returns(new RuleFilter { Type = filterType });
            var result = FilterableRuleExtensions.HasInternalFilters(this.filterableRule);

            Assert.AreEqual(result, expectation);
        }
    }
}
