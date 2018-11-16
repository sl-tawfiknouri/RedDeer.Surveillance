using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Tests.Universe
{
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
        public void Do_XDatePrecedeY_ReturnLess()
        {
            IUniverseEvent x = new UniverseEvent(UniverseStateEvent.Genesis, DateTime.UtcNow, new object());
            IUniverseEvent y = new UniverseEvent(UniverseStateEvent.Genesis, DateTime.UtcNow.AddMinutes(5), new object());

            var comparer = new UniverseEventComparer();

            var result = comparer.Compare(x, y);

            Assert.AreEqual(result, -1);
        }

        [Test]
        public void Do_XDateSucceedY_ReturnMore()
        {
            IUniverseEvent x = new UniverseEvent(UniverseStateEvent.Genesis, DateTime.UtcNow.AddMinutes(5), new object());
            IUniverseEvent y = new UniverseEvent(UniverseStateEvent.Genesis, DateTime.UtcNow, new object());

            var comparer = new UniverseEventComparer();

            var result = comparer.Compare(x, y);

            Assert.AreEqual(result, 1);
        }

        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.Unknown, 0)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.Genesis, -1)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.StockMarketOpen, -1)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.StockTickReddeer, -1)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.TradeReddeerSubmitted, -1)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.TradeReddeer, -1)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.StockMarketClose, -1)]
        [TestCase(UniverseStateEvent.Unknown, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.Unknown, 1)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.Genesis, 0)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.StockMarketOpen, -1)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.StockTickReddeer, -1)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.TradeReddeerSubmitted, -1)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.TradeReddeer, -1)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.StockMarketClose, -1)]
        [TestCase(UniverseStateEvent.Genesis, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.StockMarketOpen, UniverseStateEvent.Unknown, 1)]
        [TestCase(UniverseStateEvent.StockMarketOpen, UniverseStateEvent.Genesis, 1)]
        [TestCase(UniverseStateEvent.StockMarketOpen, UniverseStateEvent.StockMarketOpen, 0)]
        [TestCase(UniverseStateEvent.StockMarketOpen, UniverseStateEvent.StockTickReddeer, -1)]
        [TestCase(UniverseStateEvent.StockMarketOpen, UniverseStateEvent.TradeReddeerSubmitted, -1)]
        [TestCase(UniverseStateEvent.StockMarketOpen, UniverseStateEvent.TradeReddeer, -1)]
        [TestCase(UniverseStateEvent.StockMarketOpen, UniverseStateEvent.StockMarketClose, -1)]
        [TestCase(UniverseStateEvent.StockMarketOpen, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.StockTickReddeer, UniverseStateEvent.Unknown, 1)]
        [TestCase(UniverseStateEvent.StockTickReddeer, UniverseStateEvent.Genesis, 1)]
        [TestCase(UniverseStateEvent.StockTickReddeer, UniverseStateEvent.StockMarketOpen, 1)]
        [TestCase(UniverseStateEvent.StockTickReddeer, UniverseStateEvent.StockTickReddeer, 0)]
        [TestCase(UniverseStateEvent.StockTickReddeer, UniverseStateEvent.TradeReddeerSubmitted, -1)]
        [TestCase(UniverseStateEvent.StockTickReddeer, UniverseStateEvent.TradeReddeer, -1)]
        [TestCase(UniverseStateEvent.StockTickReddeer, UniverseStateEvent.StockMarketClose, -1)]
        [TestCase(UniverseStateEvent.StockTickReddeer, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.TradeReddeerSubmitted, UniverseStateEvent.Unknown, 1)]
        [TestCase(UniverseStateEvent.TradeReddeerSubmitted, UniverseStateEvent.Genesis, 1)]
        [TestCase(UniverseStateEvent.TradeReddeerSubmitted, UniverseStateEvent.StockMarketOpen, 1)]
        [TestCase(UniverseStateEvent.TradeReddeerSubmitted, UniverseStateEvent.StockTickReddeer, 1)]
        [TestCase(UniverseStateEvent.TradeReddeerSubmitted, UniverseStateEvent.TradeReddeerSubmitted, 0)]
        [TestCase(UniverseStateEvent.TradeReddeerSubmitted, UniverseStateEvent.TradeReddeer, -1)]
        [TestCase(UniverseStateEvent.TradeReddeerSubmitted, UniverseStateEvent.StockMarketClose, -1)]
        [TestCase(UniverseStateEvent.TradeReddeerSubmitted, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.TradeReddeer, UniverseStateEvent.Unknown, 1)]
        [TestCase(UniverseStateEvent.TradeReddeer, UniverseStateEvent.Genesis, 1)]
        [TestCase(UniverseStateEvent.TradeReddeer, UniverseStateEvent.StockMarketOpen, 1)]
        [TestCase(UniverseStateEvent.TradeReddeer, UniverseStateEvent.StockTickReddeer, 1)]
        [TestCase(UniverseStateEvent.TradeReddeer, UniverseStateEvent.TradeReddeerSubmitted, 1)]
        [TestCase(UniverseStateEvent.TradeReddeer, UniverseStateEvent.TradeReddeer, 0)]
        [TestCase(UniverseStateEvent.TradeReddeer, UniverseStateEvent.StockMarketClose, -1)]
        [TestCase(UniverseStateEvent.TradeReddeer, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.StockMarketClose, UniverseStateEvent.Unknown, 1)]
        [TestCase(UniverseStateEvent.StockMarketClose, UniverseStateEvent.Genesis, 1)]
        [TestCase(UniverseStateEvent.StockMarketClose, UniverseStateEvent.StockMarketOpen, 1)]
        [TestCase(UniverseStateEvent.StockMarketClose, UniverseStateEvent.StockTickReddeer, 1)]
        [TestCase(UniverseStateEvent.StockMarketClose, UniverseStateEvent.TradeReddeerSubmitted, 1)]
        [TestCase(UniverseStateEvent.StockMarketClose, UniverseStateEvent.TradeReddeer, 1)]
        [TestCase(UniverseStateEvent.StockMarketClose, UniverseStateEvent.StockMarketClose, 0)]
        [TestCase(UniverseStateEvent.StockMarketClose, UniverseStateEvent.Eschaton, -1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.Unknown, 1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.Genesis, 1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.StockMarketOpen, 1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.StockTickReddeer, 1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.TradeReddeerSubmitted, 1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.TradeReddeer, 1)]
        [TestCase(UniverseStateEvent.Eschaton, UniverseStateEvent.StockMarketClose, 1)]
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
        public void Does_ASeriesOfUniverseEvents_GetSorted_AsExpected()
        {
            var dateBase = DateTime.UtcNow;

            var x1 = new UniverseEvent(UniverseStateEvent.Unknown, dateBase.AddMinutes(2), new object());
            var x2 = new UniverseEvent(UniverseStateEvent.Eschaton, dateBase.AddMinutes(10), new object());
            var x3 = new UniverseEvent(UniverseStateEvent.Genesis, dateBase, new object());
            var x4 = new UniverseEvent(UniverseStateEvent.StockMarketClose, dateBase.AddMinutes(3), new object());
            var x5 = new UniverseEvent(UniverseStateEvent.StockMarketOpen, dateBase.AddMinutes(3), new object());
            var x6 = new UniverseEvent(UniverseStateEvent.StockMarketOpen, dateBase.AddMinutes(4), new object());
            var x7 = new UniverseEvent(UniverseStateEvent.TradeReddeer, dateBase.AddMinutes(6), new object());
            var x8 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, dateBase.AddMinutes(6), new object());
            var x9 = new UniverseEvent(UniverseStateEvent.TradeReddeer, dateBase.AddMinutes(6), new object());
            var x10 = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, dateBase.AddMinutes(6), new object());
            var x11 = new UniverseEvent(UniverseStateEvent.Eschaton, dateBase.AddMinutes(6), new object());
            var universeEvents = new List<IUniverseEvent> {x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11};

            var orderedEvents = universeEvents.OrderBy(i => i, new UniverseEventComparer()).ToList();

            foreach (var item in orderedEvents)
            {
                Console.WriteLine(item.StateChange);
            }

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
