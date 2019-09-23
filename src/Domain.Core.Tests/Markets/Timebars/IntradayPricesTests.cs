namespace Domain.Core.Tests.Markets.Timebars
{
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets.Timebars;

    using NUnit.Framework;

    [TestFixture]
    public class IntradayPricesTests
    {
        [Test]
        public void Ctor_AssignsVariables_Correctly()
        {
            var open = new Money(100, "gbp");
            var close = new Money(99, "USD");
            var high = new Money(10, "GBP");
            var low = new Money(9, "GBP");

            var intradayPrices = new IntradayPrices(open, close, high, low);

            Assert.AreEqual(open, intradayPrices.Open);
            Assert.AreEqual(close, intradayPrices.Close);
            Assert.AreEqual(high, intradayPrices.High);
            Assert.AreEqual(low, intradayPrices.Low);
        }

        [Test]
        public void Ctor_HandlesNullArgSet_WithoutThrow()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new IntradayPrices(null, null, null, null));
        }
    }
}