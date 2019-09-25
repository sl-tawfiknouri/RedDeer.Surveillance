namespace Surveillance.Engine.Rules.Tests.RuleParameters.Extensions
{
    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.RuleParameters.Extensions;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    /// <summary>
    /// The high volume filterable extensions tests.
    /// </summary>
    [TestFixture]
    public class HighVolumeFilterableExtensionsTests
    {
        /// <summary>
        /// The venue volume filterable.
        /// </summary>
        private IVenueVolumeFilterable venueVolumeFilterable;

        /// <summary>
        /// The setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.venueVolumeFilterable = A.Fake<IVenueVolumeFilterable>();
        }

        /// <summary>
        /// The has venue volume filters expected result for filter type.
        /// </summary>
        /// <param name="filterType">
        /// The filter type.
        /// </param>
        /// <param name="expectation">
        /// The expectation.
        /// </param>
        [TestCase(RuleFilterType.None, false)]
        [TestCase(RuleFilterType.Exclude, true)]
        [TestCase(RuleFilterType.Include, true)]
        public void HasVenueVolumeFiltersExpectedResultForFilterType(RuleFilterType filterType, bool expectation)
        {
            A.CallTo(() => this.venueVolumeFilterable.VenueVolumeFilter).Returns(new DecimalRangeRuleFilter { Type = filterType });

            var result = HighVolumeFilterableExtensions.HasVenueVolumeFilters(this.venueVolumeFilterable);

            Assert.AreEqual(result, expectation);
        }
    }
}
