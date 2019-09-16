namespace Surveillance.Engine.Rules.Tests.RuleParameters.Filter
{
    using System;

    using NUnit.Framework;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Engine.Rules.RuleParameters.Filter;

    /// <summary>
    /// The decimal range rule filter projector tests.
    /// </summary>
    [TestFixture]
    public class DecimalRangeRuleFilterProjectorTests
    {
        /// <summary>
        /// The project null filter returns none.
        /// </summary>
        [Test]
        public void ProjectNullFilterReturnsNone()
        {
            var projector = this.BuildProjector();

            var result = projector.Project(null);

            Assert.AreEqual(RuleFilterType.None, result.Type);
        }

        /// <summary>
        /// The project filter with lower boundary returns include with boundary.
        /// </summary>
        [Test]
        public void ProjectFilterWithLowerBoundaryReturnsIncludeWithBoundary()
        {
            var projector = this.BuildProjector();
            var range = new Range { LowerBoundary = 100 };

            var result = projector.Project(range);

            Assert.AreEqual(RuleFilterType.Include, result.Type);
            Assert.AreEqual(100, result.Min);
            Assert.AreEqual(null, result.Max);
        }

        /// <summary>
        /// The project filter with upper boundary returns include with boundary.
        /// </summary>
        [Test]
        public void ProjectFilterWithUpperBoundaryReturnsIncludeWithBoundary()
        {
            var projector = this.BuildProjector();
            var range = new Range { UpperBoundary = 100 };

            var result = projector.Project(range);

            Assert.AreEqual(RuleFilterType.Include, result.Type);
            Assert.AreEqual(null, result.Min);
            Assert.AreEqual(100, result.Max);
        }

        /// <summary>
        /// The project filter with upper and lower boundary returns include with boundary.
        /// </summary>
        [Test]
        public void ProjectFilterWithUpperAndLowerBoundaryReturnsIncludeWithBoundary()
        {
            var projector = this.BuildProjector();
            var range = new Range { UpperBoundary = 100, LowerBoundary = 99};

            var result = projector.Project(range);

            Assert.AreEqual(RuleFilterType.Include, result.Type);
            Assert.AreEqual(99, result.Min);
            Assert.AreEqual(100, result.Max);
        }

        /// <summary>
        /// The project filter with upper and lower boundary reversed throws exception.
        /// </summary>
        [Test]
        public void ProjectFilterWithUpperAndLowerBoundaryReversedThrowsException()
        {
            var projector = this.BuildProjector();
            var range = new Range { UpperBoundary = 9, LowerBoundary = 99 };

            Assert.Throws<ArgumentException>(() => projector.Project(range));
        }

        /// <summary>
        /// The build projector.
        /// </summary>
        /// <returns>
        /// The <see cref="DecimalRangeRuleFilterProjector"/>.
        /// </returns>
        private DecimalRangeRuleFilterProjector BuildProjector()
        {
            return new DecimalRangeRuleFilterProjector();
        }
    }
}
