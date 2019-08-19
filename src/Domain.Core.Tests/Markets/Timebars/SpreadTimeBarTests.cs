namespace Domain.Core.Tests.Markets.Timebars
{
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets.Timebars;
    using NUnit.Framework;

    [TestFixture]
    public class SpreadTimeBarTests
    {
        [Test]
        public void Ctor_AssignsVariables_Correctly()
        {
            var bid = new Money(100, "GBP");
            var ask = new Money(99, "GBP");
            var price = new Money(101, "GBP");
            var volume = new Volume(909);

            var spreadTimeBar = new SpreadTimeBar(bid, ask, price, volume);

            Assert.AreEqual(bid, spreadTimeBar.Bid);
            Assert.AreEqual(ask, spreadTimeBar.Ask);
            Assert.AreEqual(price, spreadTimeBar.Price);
            Assert.AreEqual(volume, spreadTimeBar.Volume);
        }
    }
}
