using Domain.Core.Trading;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Financial.Money;
using Domain.Core.Trading.Orders;

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

        [Test]
        public void Add_HasNullCollection_DoesNotThrow()
        {
            var portfolio = BuildPortfolio();

            Assert.DoesNotThrow(() => portfolio.Add((List<Order>)null));
        }

        [Test]
        public void Add_HasNullOrder_DoesNotThrow()
        {
            var portfolio = BuildPortfolio();

            Assert.DoesNotThrow(() => portfolio.Add((Order)null));
        }

        [Test]
        public void Add_NewOrder_PassedToLedger()
        {
            var portfolio = BuildPortfolio();
            var order = A.Fake<Order>();

            portfolio.Add(order);

            A.CallTo(() => _orderLedger.Add(A<Order>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Accounts_ForNoOrders_YieldsZerodFigures()
        {
            var portfolio = BuildPortfolioConcrete();

            var profitAndLoss = portfolio.ProfitAndLoss(DateTime.UtcNow, TimeSpan.FromMinutes(1));

            Assert.IsNotNull(profitAndLoss);
            Assert.IsNotEmpty(profitAndLoss);
            Assert.AreEqual(1, profitAndLoss.Count);

            var emptyProfitAndLoss = profitAndLoss.FirstOrDefault();

            Assert.AreEqual(0, emptyProfitAndLoss.Costs.Value);
            Assert.AreEqual(0, emptyProfitAndLoss.Revenue.Value);
            Assert.AreEqual(0, emptyProfitAndLoss.Profits().Value);
        }

        [Test]
        public void Accounts_ForTwoOrders_Yields200Profits()
        {
            var portfolio = BuildPortfolioConcrete();
            var startDate = new DateTime(2018, 01, 01);

            var order1 = A.Fake<Order>();
            order1.PlacedDate = startDate;
            order1.OrderDirection = OrderDirections.BUY;
            order1.OrderAverageFillPrice = new Money(10, "GBX");
            order1.OrderCurrency = new Currency("GBX");
            order1.OrderFilledVolume = 100;

            var order2 = A.Fake<Order>();
            order2.PlacedDate = startDate;
            order2.OrderDirection = OrderDirections.SELL;
            order2.OrderAverageFillPrice = new Money(12, "GBX");
            order2.OrderCurrency = new Currency("GBX");
            order2.OrderFilledVolume = 100;

            portfolio.Add(order1);
            portfolio.Add(order2);

            var profitAndLoss = portfolio.ProfitAndLoss(startDate, TimeSpan.FromMinutes(1));

            Assert.IsNotNull(profitAndLoss);
            Assert.IsNotEmpty(profitAndLoss);
            Assert.AreEqual(1, profitAndLoss.Count);

            var orderProfitAndLoss = profitAndLoss.FirstOrDefault();

            Assert.AreEqual(orderProfitAndLoss.Costs.Value, 1000);
            Assert.AreEqual(orderProfitAndLoss.Revenue.Value, 1200);
            Assert.AreEqual(orderProfitAndLoss.Profits().Value, 200);
        }

        [Test]
        public void Accounts_ForTwoOrders_YieldsZerodFigures()
        {
            var portfolio = BuildPortfolioConcrete();
            var startDate = new DateTime(2018, 01, 01);

            var order1 = A.Fake<Order>();
            order1.PlacedDate = startDate;
            order1.OrderDirection = OrderDirections.BUY;
            order1.OrderAverageFillPrice = new Money(10, "GBX");
            order1.OrderCurrency = new Currency("GBX");
            order1.OrderFilledVolume = 100;

            var order2 = A.Fake<Order>();
            order2.PlacedDate = startDate;
            order2.OrderDirection = OrderDirections.SELL;
            order2.OrderAverageFillPrice = new Money(20, "GBX");
            order2.OrderCurrency = new Currency("GBX");
            order2.OrderFilledVolume = 50;

            portfolio.Add(order1);
            portfolio.Add(order2);

            var profitAndLoss = portfolio.ProfitAndLoss(startDate, TimeSpan.FromMinutes(1));

            Assert.IsNotNull(profitAndLoss);
            Assert.IsNotEmpty(profitAndLoss);
            Assert.AreEqual(1, profitAndLoss.Count);

            var orderProfitAndLoss = profitAndLoss.FirstOrDefault();

            Assert.AreEqual(orderProfitAndLoss.Costs.Value, 1000);
            Assert.AreEqual(orderProfitAndLoss.Revenue.Value, 1000);
            Assert.AreEqual(orderProfitAndLoss.Profits().Value, 0);
        }

        private Portfolio BuildPortfolio()
        {
            return new Portfolio(_holdings, _orderLedger);
        }

        private Portfolio BuildPortfolioConcrete()
        {
            return new Portfolio(new Holdings(new List<Holding>()), new OrderLedger());
        }
    }
}
