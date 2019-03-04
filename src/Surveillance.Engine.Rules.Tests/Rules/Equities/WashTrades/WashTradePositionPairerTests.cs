using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Financial;
using Domain.Trading;
using NUnit.Framework;
using Surveillance.Engine.Rules.RuleParameters.Equities;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.WashTrade;
using Surveillance.Engine.Rules.Tests.Helpers;

namespace Surveillance.Engine.Rules.Tests.Rules.Equities.WashTrades
{
    [TestFixture]
    public class WashTradePositionPairerTests
    {
        private IWashTradeRuleEquitiesParameters _equitiesParameters;

        [SetUp]
        public void Setup()
        {
            _equitiesParameters = new WashTradeRuleEquitiesParameters(
                "id",
                TimeSpan.FromHours(8),
                true,
                true,
                true,
                null,
                null,
                null,
                null,
                10,
                0.02m,
                0.1m,
                10000,
                "GBP",
                10,
                0.03m,
                null,
                false);
        }

        [Test]
        public void PairUp_NullList_Returns_EmptyList()
        {
            var pairer = new WashTradePositionPairer();

            var results = pairer.PairUp(null, _equitiesParameters);

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void PairUp_PairsCloseBuySellPair()
        {
            var pairer = new WashTradePositionPairer();

            var buy = (new Order()).Random();
            var sell = (new Order()).Random();

            buy.OrderDirection = OrderDirections.BUY;
            sell.OrderDirection = OrderDirections.SELL;

            buy.OrderAverageFillPrice = sell.OrderAverageFillPrice;

            var tradeList = new List<Order>{ buy, sell };

            var results = pairer.PairUp(tradeList, _equitiesParameters);

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 1);

            Assert.AreEqual(results.First().Buys.Get().Count, 1);
            Assert.AreEqual(results.First().Buys.Get().First(), buy);

            Assert.AreEqual(results.First().Sells.Get().Count, 1);
            Assert.AreEqual(results.First().Sells.Get().First(), sell);
        }

        [Test]
        public void PairUp_PairsCloseBuySellPair_TwoPricePoints()
        {
            var pairer = new WashTradePositionPairer();

            var buy1 = (new Order()).Random();
            var sell1 = (new Order()).Random();

            buy1.OrderDirection = OrderDirections.BUY;
            sell1.OrderDirection = OrderDirections.SELL;
            buy1.OrderAverageFillPrice = new Money(sell1.OrderAverageFillPrice.Value.Value * 1.01m, sell1.OrderAverageFillPrice.Value.Currency);

            var buy2 = (new Order()).Random();
            var sell2 = (new Order()).Random();

            buy2.OrderDirection = OrderDirections.BUY;
            sell2.OrderDirection = OrderDirections.SELL;
            sell2.OrderAverageFillPrice = new Money(sell1.OrderAverageFillPrice.Value.Value * 1.20m, sell1.OrderAverageFillPrice.Value.Currency);
            buy2.OrderAverageFillPrice = new Money(sell2.OrderAverageFillPrice.Value.Value * 1.01m, sell2.OrderAverageFillPrice.Value.Currency);

            var tradeList = new List<Order> { buy1, sell1, buy2, sell2 };

            var results = pairer.PairUp(tradeList, _equitiesParameters);

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 2);

            Assert.AreEqual(results.First().Buys.Get().Count, 1);
            Assert.AreEqual(results.First().Buys.Get().First(), buy1);
            Assert.AreEqual(results.First().Sells.Get().Count, 1);
            Assert.AreEqual(results.First().Sells.Get().First(), sell1);

            Assert.AreEqual(results.Skip(1).First().Buys.Get().Count, 1);
            Assert.AreEqual(results.Skip(1).First().Buys.Get().First(), buy2);
            Assert.AreEqual(results.Skip(1).First().Sells.Get().Count, 1);
            Assert.AreEqual(results.Skip(1).First().Sells.Get().First(), sell2);
        }

        [Test]
        public void PairUp_PairsCloseBuySellPair_MultiDisjointPricePoints()
        {
            var pairer = new WashTradePositionPairer();

            var buy1 = (new Order()).Random();
            var sell1 = (new Order()).Random();

            buy1.OrderDirection = OrderDirections.BUY;
            sell1.OrderDirection = OrderDirections.SELL;
            buy1.OrderAverageFillPrice = new Money(sell1.OrderAverageFillPrice.Value.Value * 1.01m, sell1.OrderAverageFillPrice.Value.Currency);

            var buy2 = (new Order()).Random();
            var sell2 = (new Order()).Random();

            buy2.OrderDirection = OrderDirections.BUY;
            sell2.OrderDirection = OrderDirections.SELL;
            sell2.OrderAverageFillPrice = new Money(sell1.OrderAverageFillPrice.Value.Value * 1.20m, sell1.OrderAverageFillPrice.Value.Currency);
            buy2.OrderAverageFillPrice = new Money(sell2.OrderAverageFillPrice.Value.Value * 1.01m, sell2.OrderAverageFillPrice.Value.Currency);

            var buy3 = (new Order()).Random();
            buy3.OrderDirection = OrderDirections.BUY;
            buy3.OrderAverageFillPrice = new Money(sell1.OrderAverageFillPrice.Value.Value * 1.30m, sell2.OrderAverageFillPrice.Value.Currency);

            var buy4 = (new Order()).Random();
            buy4.OrderDirection = OrderDirections.BUY;
            buy4.OrderAverageFillPrice = new Money(sell1.OrderAverageFillPrice.Value.Value * 1.35m, sell2.OrderAverageFillPrice.Value.Currency);

            var buy5 = (new Order()).Random();
            var sell5 = (new Order()).Random();
            var sell6 = (new Order()).Random();
            var sell7 = (new Order()).Random();
            var sell8 = (new Order()).Random();
            var sell9 = (new Order()).Random();

            buy5.OrderDirection = OrderDirections.BUY;
            sell5.OrderDirection = OrderDirections.SELL;
            sell6.OrderDirection = OrderDirections.SELL;
            sell7.OrderDirection = OrderDirections.SELL;
            sell8.OrderDirection = OrderDirections.SELL;
            sell9.OrderDirection = OrderDirections.SELL;

            sell5.OrderAverageFillPrice = new Money(sell1.OrderAverageFillPrice.Value.Value * 1.40m, sell1.OrderAverageFillPrice.Value.Currency);
            sell6.OrderAverageFillPrice = new Money(sell5.OrderAverageFillPrice.Value.Value * 1.01m, sell1.OrderAverageFillPrice.Value.Currency);
            sell7.OrderAverageFillPrice = new Money(sell5.OrderAverageFillPrice.Value.Value * 1.01m, sell1.OrderAverageFillPrice.Value.Currency);
            sell8.OrderAverageFillPrice = new Money(sell5.OrderAverageFillPrice.Value.Value * 0.99m, sell1.OrderAverageFillPrice.Value.Currency);
            sell9.OrderAverageFillPrice = new Money(sell5.OrderAverageFillPrice.Value.Value, sell1.OrderAverageFillPrice.Value.Currency);
            buy5.OrderAverageFillPrice = new Money(sell5.OrderAverageFillPrice.Value.Value * 1.01m, sell2.OrderAverageFillPrice.Value.Currency);

            var tradeList = new List<Order> { buy1, sell1, buy2, sell2, buy3, buy4, buy5, sell5, sell6, sell7, sell8, sell9 };

            var results = pairer.PairUp(tradeList, _equitiesParameters);

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 3);

            Assert.AreEqual(results.First().Buys.Get().Count, 1);
            Assert.AreEqual(results.First().Buys.Get().First(), buy1);
            Assert.AreEqual(results.First().Sells.Get().Count, 1);
            Assert.AreEqual(results.First().Sells.Get().First(), sell1);

            Assert.AreEqual(results.Skip(1).First().Buys.Get().Count, 1);
            Assert.AreEqual(results.Skip(1).First().Buys.Get().First(), buy2);
            Assert.AreEqual(results.Skip(1).First().Sells.Get().Count, 1);
            Assert.AreEqual(results.Skip(1).First().Sells.Get().First(), sell2);

            Assert.AreEqual(results.Skip(2).First().Buys.Get().Count, 1);
            Assert.AreEqual(results.Skip(2).First().Buys.Get().First(), buy5);
            Assert.AreEqual(results.Skip(2).First().Sells.Get().Count, 5);

            Assert.AreEqual(results.Skip(2).First().Sells.Get().First(), sell5);
            Assert.AreEqual(results.Skip(2).First().Sells.Get().Skip(1).First(), sell6);
            Assert.AreEqual(results.Skip(2).First().Sells.Get().Skip(2).First(), sell7);
            Assert.AreEqual(results.Skip(2).First().Sells.Get().Skip(3).First(), sell8);
            Assert.AreEqual(results.Skip(2).First().Sells.Get().Skip(4).First(), sell9);
        }
    }
}
