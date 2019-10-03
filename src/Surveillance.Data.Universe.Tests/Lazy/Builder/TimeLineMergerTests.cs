namespace Surveillance.Data.Universe.Tests.Lazy.Builder
{
    using System;
    using System.Linq;

    using Domain.Core.Financial.Assets;

    using NUnit.Framework;

    using Surveillance.Data.Universe.Lazy.Builder;

    /// <summary>
    /// The time line merger tests.
    /// </summary>
    [TestFixture]
    public class TimeLineMergerTests
    {
        /// <summary>
        /// The merge null time line collection is empty.
        /// </summary>
        [Test]
        public void MergeNullTimeLineCollectionIsEmpty()
        {
            var merger = new TimeLineMerger<BmllTimeBarQuery>();

            var result = merger.Merge(null);

            Assert.IsEmpty(result);
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// The merge empty time line collection is empty.
        /// </summary>
        [Test]
        public void MergeEmptyTimeLineCollectionIsEmpty()
        {
            var merger = new TimeLineMerger<BmllTimeBarQuery>();

            var result = merger.Merge(new BmllTimeBarQuery[0]);

            Assert.IsEmpty(result);
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// The merge single time line collection returns single time line.
        /// </summary>
        [Test]
        public void MergeSingleTimeLineCollectionReturnsSingleTimeLine()
        {
            var merger = new TimeLineMerger<BmllTimeBarQuery>();
            var query = new BmllTimeBarQuery(
                new DateTime(2019, 01, 01),
                new DateTime(2019, 03, 01),
                InstrumentIdentifiers.Null());

            var result = merger.Merge(new BmllTimeBarQuery[] { query });

            Assert.IsNotNull(result);
            Assert.Contains(query, result.ToList());
            Assert.AreEqual(result.Count, 1);
        }

        /// <summary>
        /// The merge two disjointed time line collection returns two time lines.
        /// </summary>
        [Test]
        public void MergeTwoDisjointedTimeLineCollectionReturnsTwoTimeLine()
        {
            var id = InstrumentIdentifiers.Null();
            id.Sedol = "1234567";
            var merger = new TimeLineMerger<BmllTimeBarQuery>();
            var queryOne = new BmllTimeBarQuery(
                new DateTime(2019, 01, 01),
                new DateTime(2019, 03, 01),
                id);
            var queryTwo = new BmllTimeBarQuery(
                new DateTime(2019, 04, 01),
                new DateTime(2019, 05, 01),
                id);

            var result = merger.Merge(new[] { queryOne, queryTwo });

            Assert.IsNotNull(result);
            Assert.Contains(queryOne, result.ToList());
            Assert.Contains(queryTwo, result.ToList());
            Assert.AreEqual(result.Count, 2);
        }

        /// <summary>
        /// The merge two with both identical time line collection returns one time line.
        /// </summary>
        [Test]
        public void MergeTwoWithBothIdenticalTimeLineCollectionReturnsOneTimeLine()
        {
            var id = InstrumentIdentifiers.Null();
            id.Sedol = "1234567";
            var merger = new TimeLineMerger<BmllTimeBarQuery>();
            var queryOne = new BmllTimeBarQuery(
                new DateTime(2019, 01, 01),
                new DateTime(2019, 03, 01),
                id);
            var queryTwo = new BmllTimeBarQuery(
                new DateTime(2019, 01, 01),
                new DateTime(2019, 03, 01),
                id);

            var result = merger.Merge(new[] { queryOne, queryTwo });

            Assert.IsNotNull(result);
            Assert.Contains(queryOne, result.ToList());
            Assert.AreEqual(result.Count, 1);
        }

        /// <summary>
        /// The merge two with one super set time line collection returns one time line.
        /// </summary>
        [Test]
        public void MergeTwoWithOneSuperSetTimeLineCollectionReturnsOneTimeLine()
        {
            var id = InstrumentIdentifiers.Null();
            id.Sedol = "1234567";
            var merger = new TimeLineMerger<BmllTimeBarQuery>();
            var queryOne = new BmllTimeBarQuery(
                new DateTime(2019, 01, 01),
                new DateTime(2019, 03, 01),
                id);
            var queryTwo = new BmllTimeBarQuery(
                new DateTime(2019, 01, 02),
                new DateTime(2019, 02, 01),
                id);

            var result = merger.Merge(new[] { queryOne, queryTwo });

            Assert.IsNotNull(result);
            Assert.Contains(queryOne, result.ToList());
            Assert.AreEqual(result.Count, 1);
        }

        /// <summary>
        /// The merge three with one super set and one disjoint time line collection returns two time line.
        /// </summary>
        [Test]
        public void MergeThreeWithOneSuperSetAndOneDisjointTimeLineCollectionReturnsTwoTimeLine()
        {
            var id = InstrumentIdentifiers.Null();
            id.Sedol = "1234567";
            var merger = new TimeLineMerger<BmllTimeBarQuery>();
            var queryOne = new BmllTimeBarQuery(
                new DateTime(2019, 01, 01),
                new DateTime(2019, 03, 01),
                id);
            var queryTwo = new BmllTimeBarQuery(
                new DateTime(2019, 01, 02),
                new DateTime(2019, 02, 01),
                id);
            var queryThree = new BmllTimeBarQuery(
                new DateTime(2020, 01, 02),
                new DateTime(2020, 02, 01),
                id);

            var result = merger.Merge(new[] { queryOne, queryTwo, queryThree });

            Assert.IsNotNull(result);
            Assert.Contains(queryOne, result.ToList());
            Assert.Contains(queryThree, result.ToList());
            Assert.AreEqual(result.Count, 2);
        }
    }
}
