namespace Domain.Core.Tests.Markets.Timebars
{
    using System;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;

    using NUnit.Framework;

    [TestFixture]
    public class IntraDayHistoryStackTests
    {
        [Test]
        public void Add_DoesNotThrow_ForNull()
        {
            var stack = new EquityIntraDayHistoryStack(TimeSpan.Zero);

            Assert.DoesNotThrow(() => stack.Add(null, DateTime.UtcNow));
        }

        [Test]
        public void ArchiveEmptyCollection_DoesNotThrow()
        {
            var stack = new EquityIntraDayHistoryStack(TimeSpan.Zero);

            Assert.DoesNotThrow(() => stack.ActiveMarketHistory());
        }

        [Test]
        public void ArchiveEmptyCollection_OneItemToArchive_OneToKeep_ReturnsOne()
        {
            var stack = new EquityIntraDayHistoryStack(TimeSpan.Zero);
            var date = DateTime.UtcNow;
            var firstBar = date - TimeSpan.FromDays(2);
            var tb = this.GetTimeBar();
            var tb2 = this.GetTimeBar();
            var timeBarCollection = new EquityIntraDayTimeBarCollection(
                new Market("1", "xlon", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                firstBar,
                new[] { tb, tb2 });

            var timeBarCollection2 = new EquityIntraDayTimeBarCollection(
                new Market("1", "xlon", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                DateTime.UtcNow,
                new[] { tb, tb2 });

            stack.Add(timeBarCollection, firstBar);
            stack.Add(timeBarCollection2, date);

            var stackContents = stack.ActiveMarketHistory();

            Assert.AreEqual(2, stackContents.Count);

            stack.ArchiveExpiredActiveItems(date);
            var stackContentsAfterExpire = stack.ActiveMarketHistory();

            Assert.AreEqual(1, stackContentsAfterExpire.Count);
            Assert.AreEqual(timeBarCollection2, stackContentsAfterExpire.Pop());
        }

        [Test]
        public void Ctor_DoesNotThrow()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new EquityIntraDayHistoryStack(TimeSpan.Zero));
        }

        [Test]
        public void DoesExchange_ReturnNullWhen_NoPush()
        {
            var stack = new EquityIntraDayHistoryStack(TimeSpan.Zero);

            var exch = stack.Exchange();

            Assert.IsNull(exch);
        }

        [Test]
        public void DoesNotPush_IfDateDoesNotMatch()
        {
            var stack = new EquityIntraDayHistoryStack(TimeSpan.Zero);
            var date = DateTime.UtcNow;
            var tb = this.GetTimeBar();
            var timeBarCollection = new EquityIntraDayTimeBarCollection(
                new Market("1", "xlon", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                date,
                new[] { tb });

            stack.Add(timeBarCollection, date + TimeSpan.FromDays(1));

            var stackContents = stack.ActiveMarketHistory();

            Assert.IsEmpty(stackContents);
        }

        [Test]
        public void DoesPush_IfDateDoesMatch()
        {
            var stack = new EquityIntraDayHistoryStack(TimeSpan.Zero);
            var date = DateTime.UtcNow;
            var tb = this.GetTimeBar();
            var timeBarCollection = new EquityIntraDayTimeBarCollection(
                new Market("1", "xlon", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                DateTime.UtcNow,
                new[] { tb });

            stack.Add(timeBarCollection, date);

            var stackContents = stack.ActiveMarketHistory();

            Assert.AreEqual(1, stackContents.Count);
            Assert.AreEqual(timeBarCollection, stackContents.Peek());
        }

        [Test]
        public void DoesPush_IfDateDoesMatch_MultipleTimeBars()
        {
            var stack = new EquityIntraDayHistoryStack(TimeSpan.Zero);
            var date = DateTime.UtcNow;
            var tb = this.GetTimeBar();
            var tb2 = this.GetTimeBar();
            var timeBarCollection = new EquityIntraDayTimeBarCollection(
                new Market("1", "xlon", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                date,
                new[] { tb, tb2 });

            var timeBarCollection2 = new EquityIntraDayTimeBarCollection(
                new Market("1", "xlon", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                date,
                new[] { tb, tb2 });

            stack.Add(timeBarCollection, date);
            stack.Add(timeBarCollection2, date);

            var stackContents = stack.ActiveMarketHistory();

            Assert.AreEqual(2, stackContents.Count);
            Assert.AreEqual(timeBarCollection2, stackContents.Pop());
            Assert.AreEqual(timeBarCollection, stackContents.Pop());
        }

        [Test]
        public void DoesPush_SetAMarket_OnExchangeCall()
        {
            var stack = new EquityIntraDayHistoryStack(TimeSpan.Zero);
            var date = DateTime.UtcNow;
            var tb = this.GetTimeBar();
            var venue = new Market("1", "xlon", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            var timeBarCollection = new EquityIntraDayTimeBarCollection(venue, DateTime.UtcNow, new[] { tb });

            stack.Add(timeBarCollection, date);

            var exch = stack.Exchange();

            Assert.AreEqual(exch, venue);
        }

        private EquityInstrumentIntraDayTimeBar GetTimeBar()
        {
            return new EquityInstrumentIntraDayTimeBar(
                new FinancialInstrument(),
                new SpreadTimeBar(new Money(10, "GBP"), new Money(100, "GBP"), new Money(200, "GBP"), new Volume(9)),
                new DailySummaryTimeBar(
                    100,
                    "USD",
                    new IntradayPrices(null, null, null, null),
                    101,
                    new Volume(1234),
                    DateTime.UtcNow),
                DateTime.UtcNow,
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE));
        }
    }
}