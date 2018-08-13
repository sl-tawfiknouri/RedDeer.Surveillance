using Domain.Equity.Trading;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;

namespace Domain.Tests.Equity.Trading
{
    [TestFixture]
    public class UnsubscriberTests
    {
        [Test]
        public void Dispose_RemovesObserverFromBag_WhenOnlyAction()
        {
            var bag = new ConcurrentBag<IObserver<ExchangeTick>>();
            var obs1 = A.Fake<IObserver<ExchangeTick>>();
            var obs2 = A.Fake<IObserver<ExchangeTick>>();
            var obs3 = A.Fake<IObserver<ExchangeTick>>();
            bag.Add(obs1);
            bag.Add(obs2);
            bag.Add(obs3);

            var unsub = new Unsubscriber(bag, obs3);
            unsub.Dispose();

            Assert.AreEqual(bag.Count, 2);
            Assert.Contains(obs1, bag);
            Assert.Contains(obs2, bag);
        }
    }
}
