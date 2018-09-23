using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using Domain.Equity.Frames;
using Domain.Streams;

namespace Domain.Tests.Streams
{
    [TestFixture]
    public class UnsubscriberTests
    {
        [Test]
        public void Dispose_RemovesObserverFromBag_WhenOnlyAction()
        {
            var dict = new ConcurrentDictionary<IObserver<ExchangeFrame>, IObserver<ExchangeFrame>>();
            var obs1 = A.Fake<IObserver<ExchangeFrame>>();
            var obs2 = A.Fake<IObserver<ExchangeFrame>>();
            var obs3 = A.Fake<IObserver<ExchangeFrame>>();
            dict.TryAdd(obs1, obs1);
            dict.TryAdd(obs2, obs2);
            dict.TryAdd(obs3, obs3);

            var unsubscriber = new Unsubscriber<ExchangeFrame>(dict, obs3);
            unsubscriber.Dispose();

            Assert.AreEqual(2, dict.Count);
            Assert.True(dict.ContainsKey(obs1));
            Assert.True(dict.ContainsKey(obs2));
        }

        [Test]
        public void Dispose_RemovesObserverFromBag_WhenAlreadyRemoved()
        {
            var dict = new ConcurrentDictionary<IObserver<ExchangeFrame>, IObserver<ExchangeFrame>>();
            var obs1 = A.Fake<IObserver<ExchangeFrame>>();
            var obs2 = A.Fake<IObserver<ExchangeFrame>>();
            var obs3 = A.Fake<IObserver<ExchangeFrame>>();
            dict.TryAdd(obs1, obs1);
            dict.TryAdd(obs2, obs2);
            dict.TryAdd(obs3, obs3);

            var unsubscriber = new Unsubscriber<ExchangeFrame>(dict, obs3);
            dict.TryRemove(obs3, out obs3);
            unsubscriber.Dispose();

            Assert.AreEqual(2, dict.Count);
            Assert.True(dict.ContainsKey(obs1));
            Assert.True(dict.ContainsKey(obs2));
        }

        [Test]
        public void Dispose_DoesNotThrow_ForNullDictionary()
        {
            var obs1 = A.Fake<IObserver<ExchangeFrame>>();
           
            var unsubscriber = new Unsubscriber<ExchangeFrame>(null, obs1);

            Assert.DoesNotThrow(() => unsubscriber.Dispose());
        }

        [Test]
        public void Dispose_DoesThrowForNull_Observer()
        {
            var dict = new ConcurrentDictionary<IObserver<ExchangeFrame>, IObserver<ExchangeFrame>>();

            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Unsubscriber<ExchangeFrame>(dict, null));
        }
    }
}
