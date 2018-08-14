using Domain.Equity.Trading;
using NUnit.Framework;
using System;

namespace Domain.Tests.Equity.Trading
{
    [TestFixture]
    public class RingBufferTests
    {
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-1000)]
        [TestCase(-9999)]
        public void Ctor_ThrowsOutOfRangeFor_BadLimits(int limit)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RingBuffer<string>(limit));
        }

        [Test]
        public void Add_ThenRemove_ReturnsArg()
        {
            var buffer = new RingBuffer<string>(3);

            buffer.Add("Boomerang");
            var result = buffer.Remove();

            Assert.AreEqual(result, "Boomerang");
        }

        [Test]
        public void Add_InExcessOfBuffer_LosesOldestItems()
        {
            var buffer = new RingBuffer<string>(3);

            buffer.Add("str 1");
            buffer.Add("str 2");
            buffer.Add("str 3");
            buffer.Add("str 4");
            buffer.Add("str 5");

            var result1 = buffer.Remove();
            Assert.AreEqual(result1, "str 3");

            var result2 = buffer.Remove();
            Assert.AreEqual(result2, "str 4");

            var result3 = buffer.Remove();
            Assert.AreEqual(result3, "str 5");
        }

        [Test]
        public void Remove_InExcessOfAdditions_ReturnsNull()
        {
            var buffer = new RingBuffer<string>(3);

            buffer.Add("str 1");
            buffer.Add("str 2");

            var result1 = buffer.Remove();
            Assert.AreEqual(result1, "str 1");

            var result2 = buffer.Remove();
            Assert.AreEqual(result2, "str 2");

            var result3 = buffer.Remove();
            Assert.AreEqual(result3, null);
        }

    }
}
