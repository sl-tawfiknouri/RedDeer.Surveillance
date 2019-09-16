namespace Surveillance.Engine.Rules.Tests.RuleParameters.Extensions
{
    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.RuleParameters.Extensions;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    /// <summary>
    /// The reference data filterable extensions tests.
    /// </summary>
    [TestFixture]
    public class ReferenceDataFilterableExtensionsTests
    {
        /// <summary>
        /// The reference data filterable.
        /// </summary>
        private IReferenceDataFilterable referenceDataFilterable;

        /// <summary>
        /// The setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.referenceDataFilterable = A.Fake<IReferenceDataFilterable>();
        }

        /// <summary>
        /// The has reference data filters any as expected.
        /// </summary>
        [Test]
        public void HasReferenceDataFiltersAnyAsExpected()
        {
            var result = ReferenceDataFilterableExtensions.HasReferenceDataFilters(this.referenceDataFilterable);

            Assert.False(result);
        }

        /// <summary>
        /// The has reference data filters by sector as expected.
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
        public void HasReferenceDataFiltersBySectorAsExpected(RuleFilterType filterType, bool expectation)
        {
            A.CallTo(() => this.referenceDataFilterable.Sectors).Returns(new RuleFilter { Type = filterType });
            
            var result = ReferenceDataFilterableExtensions.HasReferenceDataFilters(this.referenceDataFilterable);

            Assert.AreEqual(expectation, result);
        }

        /// <summary>
        /// The has reference data filters by industries as expected.
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
        public void HasReferenceDataFiltersByIndustriesAsExpected(RuleFilterType filterType, bool expectation)
        {
            A.CallTo(() => this.referenceDataFilterable.Industries).Returns(new RuleFilter { Type = filterType });

            var result = ReferenceDataFilterableExtensions.HasReferenceDataFilters(this.referenceDataFilterable);

            Assert.AreEqual(expectation, result);
        }

        /// <summary>
        /// The has reference data filters by regions as expected.
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
        public void HasReferenceDataFiltersByRegionsAsExpected(RuleFilterType filterType, bool expectation)
        {
            A.CallTo(() => this.referenceDataFilterable.Regions).Returns(new RuleFilter { Type = filterType });

            var result = ReferenceDataFilterableExtensions.HasReferenceDataFilters(this.referenceDataFilterable);

            Assert.AreEqual(expectation, result);
        }

        /// <summary>
        /// The has reference data filters by countries as expected.
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
        public void HasReferenceDataFiltersByCountriesAsExpected(RuleFilterType filterType, bool expectation)
        {
            A.CallTo(() => this.referenceDataFilterable.Countries).Returns(new RuleFilter { Type = filterType });

            var result = ReferenceDataFilterableExtensions.HasReferenceDataFilters(this.referenceDataFilterable);

            Assert.AreEqual(expectation, result);
        }
    }
}
