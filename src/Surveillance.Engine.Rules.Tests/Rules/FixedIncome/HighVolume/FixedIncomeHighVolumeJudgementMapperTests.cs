namespace Surveillance.Engine.Rules.Tests.Rules.FixedIncome.HighVolume
{
    using System;

    using Domain.Core.Financial.Assets.Interfaces;
    using Domain.Surveillance.Judgement.FixedIncome;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.Judgements.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume;

    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// The fixed income high volume judgement mapper tests.
    /// </summary>
    [TestFixture]
    public class FixedIncomeHighVolumeJudgementMapperTests
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<FixedIncomeHighVolumeJudgementMapper> logger;

        /// <summary>
        /// The context.
        /// </summary>
        private IFixedIncomeHighVolumeJudgementContext context;

        /// <summary>
        /// The financial instrument.
        /// </summary>
        private IFinancialInstrument financialInstrument;

        /// <summary>
        /// The constructor throws argument null exception for null logger.
        /// </summary>
        [Test]
        public void ConstructorThrowsArgumentNullExceptionForNullLogger()
        {
            Assert.Throws<ArgumentNullException>(() => new FixedIncomeHighVolumeJudgementMapper(null));
        }

        /// <summary>
        /// The build description for window breach returns expected description.
        /// </summary>
        [Test]
        public void BuildDescriptionForWindowBreachReturnsExpectedDescription()
        {
            var mapper = this.BuildMapper();
            A.CallTo(() => this.context.IsIssuanceBreach).Returns(false);
            A.CallTo(() => this.financialInstrument.Name).Returns("ryan-test-security");

            var windowAnalysis =
                new FixedIncomeHighVolumeJudgement.BreachDetails(
                    100, 
                    0.2m, 
                    300, 
                    0.6m, 
                    true);

            A.CallTo(() => this.context.Judgement.WindowAnalysisAnalysis).Returns(windowAnalysis);

            var description = mapper.BuildDescription(this.context);

            Assert.AreEqual("Fixed Income High Volume rule breach detected for ryan-test-security. Percentage of window volume breach has occured. A window volume limit of 20% was exceeded by trading 60% of window volume within the window of 0 minutes at the venue () . 300 volume was the allocated fill against a breach threshold volume of 100.", description);
        }

        /// <summary>
        /// The build description for daily breach returns expected description.
        /// </summary>
        [Test]
        public void BuildDescriptionForDailyBreachReturnsExpectedDescription()
        {
            var mapper = this.BuildMapper();
            A.CallTo(() => this.context.IsIssuanceBreach).Returns(false);
            A.CallTo(() => this.financialInstrument.Name).Returns("ryan-test-security");

            var dailyAnalysis =
                new FixedIncomeHighVolumeJudgement.BreachDetails(
                    69,
                    0.1m,
                    230,
                    0.3m,
                    true);

            A.CallTo(() => this.context.Judgement.DailyAnalysisAnalysis).Returns(dailyAnalysis);

            var description = mapper.BuildDescription(this.context);

            Assert.AreEqual("Fixed Income High Volume rule breach detected for ryan-test-security. Percentage of daily volume breach has occured. A daily volume limit of 10% was exceeded by trading 30% of daily volume at the venue () . 230 volume was the allocated fill against a breach threshold volume of 69.", description);
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.logger = new NullLogger<FixedIncomeHighVolumeJudgementMapper>();
            this.context = A.Fake<IFixedIncomeHighVolumeJudgementContext>();
            this.financialInstrument = A.Fake<IFinancialInstrument>();
            A.CallTo(() => this.context.Security).Returns(this.financialInstrument);
        }

        /// <summary>
        /// The build mapper.
        /// </summary>
        /// <returns>
        /// The <see cref="FixedIncomeHighVolumeJudgementMapper"/>.
        /// </returns>
        private FixedIncomeHighVolumeJudgementMapper BuildMapper()
        {
            return new FixedIncomeHighVolumeJudgementMapper(new NullLogger<FixedIncomeHighVolumeJudgementMapper>());
        }
    }
}
