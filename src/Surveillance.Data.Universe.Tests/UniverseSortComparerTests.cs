namespace Surveillance.Data.Universe.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using Surveillance.Data.Universe.Interfaces;

    [TestFixture]
    public class UniverseSortComparerTests
    {
        [Test]
        public void Do_TwoNullEvents_ReturnEqual()
        {
            IUniverseEvent x = null;
            IUniverseEvent y = null;
            var comparer = new UniverseEventComparer();

            var result = comparer.Compare(x, y);

            Assert.AreEqual(result, 0);
        }

        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.Unknown, 0)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.Genesis, 0)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.ExchangeOpen, -1)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.EquityIntraDayTick, -1)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.OrderPlaced, -1)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.Order, -1)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.ExchangeClose, -1)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.Unknown, 0)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.Genesis, 0)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.ExchangeOpen, -1)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.EquityIntraDayTick, -1)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.OrderPlaced, -1)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.Order, -1)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.ExchangeClose, -1)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.ExchangeOpen, UniverseStateEvent.Unknown, 1)]
        [TestCase(UniverseStateEvent.ExchangeOpen, UniverseStateEvent.Genesis, 1)]
        [TestCase(UniverseStateEvent.ExchangeOpen, UniverseStateEvent.ExchangeOpen, 0)]
        [TestCase(UniverseStateEvent.ExchangeOpen, UniverseStateEvent.EquityIntraDayTick, -1)]
        [TestCase(UniverseStateEvent.ExchangeOpen, UniverseStateEvent.OrderPlaced, -1)]
        [TestCase(UniverseStateEvent.ExchangeOpen, UniverseStateEvent.Order, -1)]
        [TestCase(UniverseStateEvent.ExchangeOpen, UniverseStateEvent.ExchangeClose, -1)]
        [TestCase(UniverseStateEvent.ExchangeOpen, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.EquityIntraDayTick, UniverseStateEvent.Unknown, 1)]
        [TestCase(UniverseStateEvent.EquityIntraDayTick, UniverseStateEvent.Genesis, 1)]
        [TestCase(UniverseStateEvent.EquityIntraDayTick, UniverseStateEvent.ExchangeOpen, 1)]
        [TestCase(UniverseStateEvent.EquityIntraDayTick, UniverseStateEvent.EquityIntraDayTick, 0)]
        [TestCase(UniverseStateEvent.EquityIntraDayTick, UniverseStateEvent.OrderPlaced, -1)]
        [TestCase(UniverseStateEvent.EquityIntraDayTick, UniverseStateEvent.Order, -1)]
        [TestCase(UniverseStateEvent.EquityIntraDayTick, UniverseStateEvent.ExchangeClose, -1)]
        [TestCase(UniverseStateEvent.EquityIntraDayTick, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.OrderPlaced, UniverseStateEvent.Unknown, 1)]
        [TestCase(UniverseStateEvent.OrderPlaced, UniverseStateEvent.Genesis, 1)]
        [TestCase(UniverseStateEvent.OrderPlaced, UniverseStateEvent.ExchangeOpen, 1)]
        [TestCase(UniverseStateEvent.OrderPlaced, UniverseStateEvent.EquityIntraDayTick, 1)]
        [TestCase(UniverseStateEvent.OrderPlaced, UniverseStateEvent.OrderPlaced, 0)]
        [TestCase(UniverseStateEvent.OrderPlaced, UniverseStateEvent.Order, -1)]
        [TestCase(UniverseStateEvent.OrderPlaced, UniverseStateEvent.ExchangeClose, -1)]
        [TestCase(UniverseStateEvent.OrderPlaced, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.Order, UniverseStateEvent.Unknown, 1)]
        [TestCase(UniverseStateEvent.Order, UniverseStateEvent.Genesis, 1)]
        [TestCase(UniverseStateEvent.Order, UniverseStateEvent.ExchangeOpen, 1)]
        [TestCase(UniverseStateEvent.Order, UniverseStateEvent.EquityIntraDayTick, 1)]
        [TestCase(UniverseStateEvent.Order, UniverseStateEvent.OrderPlaced, 1)]
        [TestCase(UniverseStateEvent.Order, UniverseStateEvent.Order, 0)]
        [TestCase(UniverseStateEvent.Order, UniverseStateEvent.ExchangeClose, -1)]
        [TestCase(UniverseStateEvent.Order, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.ExchangeClose, UniverseStateEvent.Unknown, 1)]
        [TestCase(UniverseStateEvent.ExchangeClose, UniverseStateEvent.Genesis, 1)]
        [TestCase(UniverseStateEvent.ExchangeClose, UniverseStateEvent.ExchangeOpen, 1)]
        [TestCase(UniverseStateEvent.ExchangeClose, UniverseStateEvent.EquityIntraDayTick, 1)]
        [TestCase(UniverseStateEvent.ExchangeClose, UniverseStateEvent.OrderPlaced, 1)]
        [TestCase(UniverseStateEvent.ExchangeClose, UniverseStateEvent.Order, 1)]
        [TestCase(UniverseStateEvent.ExchangeClose, UniverseStateEvent.ExchangeClose, 0)]
        [TestCase(UniverseStateEvent.ExchangeClose, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.Unknown, 1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.Genesis, 1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.ExchangeOpen, 1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.EquityIntraDayTick, 1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.OrderPlaced, 1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.Order, 1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.ExchangeClose, 1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.Eschaton, 0)]
        public void Do_X_ReturnExpected_ToY_OnSameDate(UniverseStateEvent xe, UniverseStateEvent ye, int expected)
        {
            var eventDate = DateTime.UtcNow;
            var x = new UniverseEvent(xe, eventDate, new object());
            var y = new UniverseEvent(ye, eventDate, new object());

            var comparer = new UniverseEventComparer();

            var result = comparer.Compare(x, y);

            Assert.AreEqual(result, expected);
        }

        [Test]
        public void Do_XDatePrecedeY_ReturnLess()
        {
            IUniverseEvent x = new UniverseEvent(UniverseStateEvent.Genesis, DateTime.UtcNow, new object());
            IUniverseEvent y = new UniverseEvent(
                UniverseStateEvent.Genesis,
                DateTime.UtcNow.AddMinutes(5),
                new object());

            var comparer = new UniverseEventComparer();

            var result = comparer.Compare(x, y);

            Assert.AreEqual(result, -1);
        }

        [Test]
        public void Do_XDateSucceedY_ReturnMore()
        {
            IUniverseEvent x = new UniverseEvent(
                UniverseStateEvent.Genesis,
                DateTime.UtcNow.AddMinutes(5),
                new object());
            IUniverseEvent y = new UniverseEvent(UniverseStateEvent.Genesis, DateTime.UtcNow, new object());

            var comparer = new UniverseEventComparer();

            var result = comparer.Compare(x, y);

            Assert.AreEqual(result, 1);
        }

        [Test]
        public void Do_XNullEvent_ReturnLess()
        {
            IUniverseEvent x = null;
            IUniverseEvent y = new UniverseEvent(UniverseStateEvent.Genesis, DateTime.UtcNow, new object());
            var comparer = new UniverseEventComparer();

            var result = comparer.Compare(x, y);

            Assert.AreEqual(result, -1);
        }

        [Test]
        public void Do_YNullEvent_ReturnMore()
        {
            IUniverseEvent x = new UniverseEvent(UniverseStateEvent.Genesis, DateTime.UtcNow, new object());
            IUniverseEvent y = null;

            var comparer = new UniverseEventComparer();

            var result = comparer.Compare(x, y);

            Assert.AreEqual(result, 1);
        }

        [Test]
        public void Does_ASeriesOfUniverseEvents_GetSorted_AsExpected()
        {
            var dateBase = DateTime.UtcNow;

            var x1 = new UniverseEvent(UniverseStateEvent.Unknown, dateBase.AddMinutes(2), new object());
            var x2 = new UniverseEvent(UniverseStateEvent.Eschaton, dateBase.AddMinutes(10), new object());
            var x3 = new UniverseEvent(UniverseStateEvent.Genesis, dateBase, new object());
            var x4 = new UniverseEvent(UniverseStateEvent.ExchangeClose, dateBase.AddMinutes(3), new object());
            var x5 = new UniverseEvent(UniverseStateEvent.ExchangeOpen, dateBase.AddMinutes(3), new object());
            var x6 = new UniverseEvent(UniverseStateEvent.ExchangeOpen, dateBase.AddMinutes(4), new object());
            var x7 = new UniverseEvent(UniverseStateEvent.Order, dateBase.AddMinutes(6), new object());
            var x8 = new UniverseEvent(UniverseStateEvent.EquityIntraDayTick, dateBase.AddMinutes(6), new object());
            var x9 = new UniverseEvent(UniverseStateEvent.Order, dateBase.AddMinutes(6), new object());
            var x10 = new UniverseEvent(UniverseStateEvent.OrderPlaced, dateBase.AddMinutes(6), new object());
            var x11 = new UniverseEvent(UniverseStateEvent.Eschaton, dateBase.AddMinutes(6), new object());
            var universeEvents = new List<IUniverseEvent>
                                     {
                                         x1,
                                         x2,
                                         x3,
                                         x4,
                                         x5,
                                         x6,
                                         x7,
                                         x8,
                                         x9,
                                         x10,
                                         x11
                                     };

            var orderedEvents = universeEvents.OrderBy(i => i, new UniverseEventComparer()).ToList();

            foreach (var item in orderedEvents) Console.WriteLine(item.StateChange);

            Assert.AreEqual(orderedEvents.First(), x3);
            Assert.AreEqual(orderedEvents.Skip(1).First(), x1);
            Assert.AreEqual(orderedEvents.Skip(2).First(), x5);
            Assert.AreEqual(orderedEvents.Skip(3).First(), x4);
            Assert.AreEqual(orderedEvents.Skip(4).First(), x6);
            Assert.AreEqual(orderedEvents.Skip(5).First(), x8);
            Assert.AreEqual(orderedEvents.Skip(6).First(), x10);
            Assert.AreEqual(orderedEvents.Skip(7).First(), x7);
            Assert.AreEqual(orderedEvents.Skip(8).First(), x9);
            Assert.AreEqual(orderedEvents.Skip(9).First(), x11);
            Assert.AreEqual(orderedEvents.Skip(10).First(), x2);
        }
    }
}