using Domain.Core.Trading;
using FakeItEasy;
using NUnit.Framework;
using System;

namespace Domain.Core.Tests.Trading
{
    [TestFixture]
    public class PortfolioTests
    {
        private IHoldings _holdings;
        private IOrderLedger _orderLedger;

        [SetUp]
        public void Setup()
        {
            _holdings = A.Fake<IHoldings>();
            _orderLedger = A.Fake<IOrderLedger>();
        }

        [Test]
        public void Constructor_HasNullHolding_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new Portfolio(null, _orderLedger));
        }

        [Test]
        public void Constructor_HasNullOrderLedger_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new Portfolio(_holdings, null));
        }
    }
}
