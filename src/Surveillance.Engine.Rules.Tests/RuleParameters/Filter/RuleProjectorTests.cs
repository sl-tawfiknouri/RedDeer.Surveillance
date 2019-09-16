namespace Surveillance.Engine.Rules.Tests.RuleParameters.Filter
{
    using NUnit.Framework;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Engine.Rules.RuleParameters.Filter;

    /// <summary>
    /// The rule projector tests.
    /// </summary>
    [TestFixture]
    public class RuleProjectorTests
    {
        /// <summary>
        /// The project null filter is none rule filter.
        /// </summary>
        [Test]
        public void ProjectNullFilterIsNoneRuleFilter()
        {
            var projector = this.BuildProjector();

            var result = projector.Project(null);

            Assert.IsNotNull(result);
            Assert.AreEqual(RuleFilterType.None, result.Type);
        }

        /// <summary>
        /// The project none filter is none rule filter.
        /// </summary>
        [Test]
        public void ProjectNoneFilterIsNoneRuleFilter()
        {
            var projector = this.BuildProjector();
            var filter = new Filter { Type = FilterType.None };

            var result = projector.Project(filter);

            Assert.IsNotNull(result);
            Assert.AreEqual(RuleFilterType.None, result.Type);
        }

        /// <summary>
        /// The project include filter is include rule filter.
        /// </summary>
        [Test]
        public void ProjectIncludeFilterIsIncludeRuleFilter()
        {
            var projector = this.BuildProjector();
            var filter = new Filter { Type = FilterType.Include };

            var result = projector.Project(filter);

            Assert.IsNotNull(result);
            Assert.AreEqual(RuleFilterType.Include, result.Type);
        }

        /// <summary>
        /// The project exclude filter is exclude rule filter.
        /// </summary>
        [Test]
        public void ProjectExcludeFilterIsExcludeRuleFilter()
        {
            var projector = this.BuildProjector();
            var filter = new Filter { Type = FilterType.Exclude, Ids = new[] { "a" } };

            var result = projector.Project(filter);

            Assert.IsNotNull(result);
            Assert.AreEqual(RuleFilterType.Exclude, result.Type);
            Assert.AreEqual(new[] { "a" }, result.Ids);
        }

        /// <summary>
        /// The build projector.
        /// </summary>
        /// <returns>
        /// The <see cref="RuleFilterProjector"/>.
        /// </returns>
        private RuleFilterProjector BuildProjector()
        {
            return new RuleFilterProjector();
        }
    }
}
