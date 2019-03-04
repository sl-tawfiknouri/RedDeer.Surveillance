using System;
using Domain.Core.Financial;
using NUnit.Framework;

namespace Domain.Tests.Financial
{
    [TestFixture]
    public class DateRangeTests
    {
        [Test]
        public void Does_Length_Return_Expected_TimeSpan()
        {
            var dr = new DateRange(DateTime.Parse("2018/01/01"), DateTime.Parse("2018/01/02"));

            Assert.AreEqual(dr.Length, TimeSpan.FromDays(1));
        }

        [Test]
        public void Does_Length_Return_Expected_TimeSpan_Bad_Order()
        {
            Assert.Throws<ArgumentNullException>(() => new DateRange(DateTime.Parse("2018/01/03"), DateTime.Parse("2018/01/02")));
        }

        [Test]
        public void Does_Intersection_With_Intersection_Return_True()
        {
            var dr1 = new DateRange(DateTime.Parse("2018/01/01"), DateTime.Parse("2018/01/03"));
            var dr2 = new DateRange(DateTime.Parse("2018/01/01"), DateTime.Parse("2018/01/02"));

            Assert.IsTrue(dr1.Intersection(dr2));
        }

        [Test]
        public void Does_Intersection_With_Inverted_Intersection_Return_True()
        {
            var dr1 = new DateRange(DateTime.Parse("2018/01/01"), DateTime.Parse("2018/01/03"));
            var dr2 = new DateRange(DateTime.Parse("2018/01/01"), DateTime.Parse("2018/01/02"));

            Assert.IsTrue(dr2.Intersection(dr1));
        }

        [Test]
        public void Does_Intersection_With_Wider_First_Return_True()
        {
            var dr1 = new DateRange(DateTime.Parse("2018/01/01"), DateTime.Parse("2018/01/10"));
            var dr2 = new DateRange(DateTime.Parse("2018/01/02"), DateTime.Parse("2018/01/02"));

            Assert.IsTrue(dr1.Intersection(dr2));
        }

        [Test]
        public void Does_Intersection_With_Wider_Second_Return_True()
        {
            var dr1 = new DateRange(DateTime.Parse("2018/01/01"), DateTime.Parse("2018/01/10"));
            var dr2 = new DateRange(DateTime.Parse("2018/01/02"), DateTime.Parse("2018/01/02"));

            Assert.IsTrue(dr2.Intersection(dr1));
        }

        [Test]
        public void Does_Intersection_With_Non_Intersecting_Dates_Return_False()
        {
            var dr1 = new DateRange(DateTime.Parse("2017/01/01"), DateTime.Parse("2017/01/10"));
            var dr2 = new DateRange(DateTime.Parse("2018/01/02"), DateTime.Parse("2018/01/02"));

            Assert.IsFalse(dr1.Intersection(dr2));
        }

        [Test]
        public void Does_Intersection_With_Non_Intersecting_Dates_Inverted_Return_False()
        {
            var dr1 = new DateRange(DateTime.Parse("2018/01/01"), DateTime.Parse("2018/01/10"));
            var dr2 = new DateRange(DateTime.Parse("2017/01/02"), DateTime.Parse("2017/01/02"));

            Assert.IsFalse(dr1.Intersection(dr2));
        }
    }
}
