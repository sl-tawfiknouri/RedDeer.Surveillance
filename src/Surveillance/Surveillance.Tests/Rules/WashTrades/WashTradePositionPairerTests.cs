using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Financial;
using DomainV2.Trading;
using NUnit.Framework;
using Surveillance.RuleParameters;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.WashTrade;
using Surveillance.Tests.Helpers;

namespace Surveillance.Tests.Rules.WashTrades
{
    [TestFixture]
    public class WashTradePositionPairerTests
    {
        private IWashTradeRuleParameters _parameters;

        [SetUp]
        public void Setup()
        {
            _parameters = new WashTradeRuleParameters(
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

            var results = pairer.PairUp(null, _parameters);

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void PairUp_PairsCloseBuySellPair()
        {
            var pairer = new WashTradePositionPairer();

            var buy = (new Order()).Random();
            var sell = (new Order()).Random();

            buy.OrderPosition = OrderPositions.BUY;
            sell.OrderPosition = OrderPositions.SELL;

            buy.OrderAveragePrice = sell.OrderAveragePrice;

            var tradeList = new List<Order>{ buy, sell };

            var results = pairer.PairUp(tradeList, _parameters);

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

            buy1.OrderPosition = OrderPositions.BUY;
            sell1.OrderPosition = OrderPositions.SELL;
            buy1.OrderAveragePrice = new CurrencyAmount(sell1.OrderAveragePrice.Value.Value * 1.01m, sell1.OrderAveragePrice.Value.Currency);

            var buy2 = (new Order()).Random();
            var sell2 = (new Order()).Random();

            buy2.OrderPosition = OrderPositions.BUY;
            sell2.OrderPosition = OrderPositions.SELL;
            sell2.OrderAveragePrice = new CurrencyAmount(sell1.OrderAveragePrice.Value.Value * 1.20m, sell1.OrderAveragePrice.Value.Currency);
            buy2.OrderAveragePrice = new CurrencyAmount(sell2.OrderAveragePrice.Value.Value * 1.01m, sell2.OrderAveragePrice.Value.Currency);

            var tradeList = new List<Order> { buy1, sell1, buy2, sell2 };

            var results = pairer.PairUp(tradeList, _parameters);

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

            buy1.OrderPosition = OrderPositions.BUY;
            sell1.OrderPosition = OrderPositions.SELL;
            buy1.OrderAveragePrice = new CurrencyAmount(sell1.OrderAveragePrice.Value.Value * 1.01m, sell1.OrderAveragePrice.Value.Currency);

            var buy2 = (new Order()).Random();
            var sell2 = (new Order()).Random();

            buy2.OrderPosition = OrderPositions.BUY;
            sell2.OrderPosition = OrderPositions.SELL;
            sell2.OrderAveragePrice = new CurrencyAmount(sell1.OrderAveragePrice.Value.Value * 1.20m, sell1.OrderAveragePrice.Value.Currency);
            buy2.OrderAveragePrice = new CurrencyAmount(sell2.OrderAveragePrice.Value.Value * 1.01m, sell2.OrderAveragePrice.Value.Currency);

            var buy3 = (new Order()).Random();
            buy3.OrderPosition = OrderPositions.BUY;
            buy3.OrderAveragePrice = new CurrencyAmount(sell1.OrderAveragePrice.Value.Value * 1.30m, sell2.OrderAveragePrice.Value.Currency);

            var buy4 = (new Order()).Random();
            buy4.OrderPosition = OrderPositions.BUY;
            buy4.OrderAveragePrice = new CurrencyAmount(sell1.OrderAveragePrice.Value.Value * 1.35m, sell2.OrderAveragePrice.Value.Currency);

            var buy5 = (new Order()).Random();
            var sell5 = (new Order()).Random();
            var sell6 = (new Order()).Random();
            var sell7 = (new Order()).Random();
            var sell8 = (new Order()).Random();
            var sell9 = (new Order()).Random();

            buy5.OrderPosition = OrderPositions.BUY;
            sell5.OrderPosition = OrderPositions.SELL;
            sell6.OrderPosition = OrderPositions.SELL;
            sell7.OrderPosition = OrderPositions.SELL;
            sell8.OrderPosition = OrderPositions.SELL;
            sell9.OrderPosition = OrderPositions.SELL;

            sell5.OrderAveragePrice = new CurrencyAmount(sell1.OrderAveragePrice.Value.Value * 1.40m, sell1.OrderAveragePrice.Value.Currency);
            sell6.OrderAveragePrice = new CurrencyAmount(sell5.OrderAveragePrice.Value.Value * 1.01m, sell1.OrderAveragePrice.Value.Currency);
            sell7.OrderAveragePrice = new CurrencyAmount(sell5.OrderAveragePrice.Value.Value * 1.01m, sell1.OrderAveragePrice.Value.Currency);
            sell8.OrderAveragePrice = new CurrencyAmount(sell5.OrderAveragePrice.Value.Value * 0.99m, sell1.OrderAveragePrice.Value.Currency);
            sell9.OrderAveragePrice = new CurrencyAmount(sell5.OrderAveragePrice.Value.Value, sell1.OrderAveragePrice.Value.Currency);
            buy5.OrderAveragePrice = new CurrencyAmount(sell5.OrderAveragePrice.Value.Value * 1.01m, sell2.OrderAveragePrice.Value.Currency);

            var tradeList = new List<Order> { buy1, sell1, buy2, sell2, buy3, buy4, buy5, sell5, sell6, sell7, sell8, sell9 };

            var results = pairer.PairUp(tradeList, _parameters);

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
