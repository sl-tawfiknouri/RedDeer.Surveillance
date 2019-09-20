namespace Surveillance.Engine.Rules.Tests.Rules.FixedIncome.HighVolume
{
    using System;

    using Domain.Core.Markets;
    using Domain.Core.Trading.Interfaces;
    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume;

    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// The fixed income high volume context tests.
    /// </summary>
    [TestFixture]
    public class FixedIncomeHighVolumeJudgementContextTests
    {
        /// <summary>
        /// The judgement.
        /// </summary>
        private IFixedIncomeHighVolumeJudgement judgement;

        /// <summary>
        /// The trade position.
        /// </summary>
        private ITradePosition tradePosition;

        /// <summary>
        /// The market.
        /// </summary>
        private Market market;

        /// <summary>
        /// The constructor null judgement throws argument null exception.
        /// </summary>
        [Test]
        public void ConstructorNullJudgementThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new FixedIncomeHighVolumeJudgementContext(
                    null, 
                    true, 
                    this.tradePosition, 
                    this.market));
        }

        /// <summary>
        /// The constructor null trade position throws argument null exception.
        /// </summary>
        [Test]
        public void ConstructorNullTradePositionThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new FixedIncomeHighVolumeJudgementContext(
                    this.judgement,
                    true,
                    null,
                    this.market));
        }

        /// <summary>
        /// The setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.tradePosition = A.Fake<ITradePosition>();
            this.judgement = A.Fake<IFixedIncomeHighVolumeJudgement>();
            this.market = new Market("id-1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
        }
    }
}
