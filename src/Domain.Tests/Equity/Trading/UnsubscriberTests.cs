﻿using Domain.Equity.Trading;
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
            var dict = new ConcurrentDictionary<IObserver<ExchangeTick>, IObserver<ExchangeTick>>();
            var obs1 = A.Fake<IObserver<ExchangeTick>>();
            var obs2 = A.Fake<IObserver<ExchangeTick>>();
            var obs3 = A.Fake<IObserver<ExchangeTick>>();
            dict.TryAdd(obs1, obs1);
            dict.TryAdd(obs2, obs2);
            dict.TryAdd(obs3, obs3);

            var unsub = new Unsubscriber(dict, obs3);
            unsub.Dispose();

            Assert.AreEqual(2, dict.Count);
            Assert.True(dict.ContainsKey(obs1));
            Assert.True(dict.ContainsKey(obs2));
        }

        [Test]
        public void Dispose_RemovesObserverFromBag_WhenAlreadyRemoved()
        {
            var dict = new ConcurrentDictionary<IObserver<ExchangeTick>, IObserver<ExchangeTick>>();
            var obs1 = A.Fake<IObserver<ExchangeTick>>();
            var obs2 = A.Fake<IObserver<ExchangeTick>>();
            var obs3 = A.Fake<IObserver<ExchangeTick>>();
            dict.TryAdd(obs1, obs1);
            dict.TryAdd(obs2, obs2);
            dict.TryAdd(obs3, obs3);

            var unsub = new Unsubscriber(dict, obs3);
            dict.TryRemove(obs3, out obs3);
            unsub.Dispose();

            Assert.AreEqual(2, dict.Count);
            Assert.True(dict.ContainsKey(obs1));
            Assert.True(dict.ContainsKey(obs2));
        }

        [Test]
        public void Dispose_DoesNotThrow_ForNullDictionary()
        {
            var obs1 = A.Fake<IObserver<ExchangeTick>>();
           
            var unsub = new Unsubscriber(null, obs1);

            Assert.DoesNotThrow(() => unsub.Dispose());
        }

        [Test]
        public void Dispose_DoesThrowForNull_Observer()
        {
            var dict = new ConcurrentDictionary<IObserver<ExchangeTick>, IObserver<ExchangeTick>>();

            Assert.Throws<ArgumentNullException>(() => new Unsubscriber(dict, null));
        }
    }
}
