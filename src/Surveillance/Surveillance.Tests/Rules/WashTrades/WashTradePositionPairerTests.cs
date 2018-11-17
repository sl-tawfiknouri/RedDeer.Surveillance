using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity;
using Domain.Trades.Orders;
using NUnit.Framework;
using Surveillance.Rules.WashTrade;
using Surveillance.Rule_Parameters;
using Surveillance.Rule_Parameters.Interfaces;
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
            _parameters = new WashTradeRuleParameters(TimeSpan.FromHours(8),true,true,true,null , null, null, null, 10, 0.02m, 0.1m, 10000, "GBP");
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

            var buy = (new TradeOrderFrame()).Random();
            var sell = (new TradeOrderFrame()).Random();

            buy.Position = OrderPosition.Buy;
            sell.Position = OrderPosition.Sell;

            buy.ExecutedPrice = sell.ExecutedPrice;

            var tradeList = new List<TradeOrderFrame>{ buy, sell };

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

            var buy1 = (new TradeOrderFrame()).Random();
            var sell1 = (new TradeOrderFrame()).Random();

            buy1.Position = OrderPosition.Buy;
            sell1.Position = OrderPosition.Sell;
            buy1.ExecutedPrice = new Price(sell1.ExecutedPrice.Value.Value * 1.01m, sell1.ExecutedPrice.Value.Currency);

            var buy2 = (new TradeOrderFrame()).Random();
            var sell2 = (new TradeOrderFrame()).Random();

            buy2.Position = OrderPosition.Buy;
            sell2.Position = OrderPosition.Sell;
            sell2.ExecutedPrice = new Price(sell1.ExecutedPrice.Value.Value * 1.20m, sell1.ExecutedPrice.Value.Currency);
            buy2.ExecutedPrice = new Price(sell2.ExecutedPrice.Value.Value * 1.01m, sell2.ExecutedPrice.Value.Currency);

            var tradeList = new List<TradeOrderFrame> { buy1, sell1, buy2, sell2 };

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

            var buy1 = (new TradeOrderFrame()).Random();
            var sell1 = (new TradeOrderFrame()).Random();

            buy1.Position = OrderPosition.Buy;
            sell1.Position = OrderPosition.Sell;
            buy1.ExecutedPrice = new Price(sell1.ExecutedPrice.Value.Value * 1.01m, sell1.ExecutedPrice.Value.Currency);

            var buy2 = (new TradeOrderFrame()).Random();
            var sell2 = (new TradeOrderFrame()).Random();

            buy2.Position = OrderPosition.Buy;
            sell2.Position = OrderPosition.Sell;
            sell2.ExecutedPrice = new Price(sell1.ExecutedPrice.Value.Value * 1.20m, sell1.ExecutedPrice.Value.Currency);
            buy2.ExecutedPrice = new Price(sell2.ExecutedPrice.Value.Value * 1.01m, sell2.ExecutedPrice.Value.Currency);

            var buy3 = (new TradeOrderFrame()).Random();
            buy3.Position = OrderPosition.Buy;
            buy3.ExecutedPrice = new Price(sell1.ExecutedPrice.Value.Value * 1.30m, sell2.ExecutedPrice.Value.Currency);

            var buy4 = (new TradeOrderFrame()).Random();
            buy4.Position = OrderPosition.Buy;
            buy4.ExecutedPrice = new Price(sell1.ExecutedPrice.Value.Value * 1.35m, sell2.ExecutedPrice.Value.Currency);

            var buy5 = (new TradeOrderFrame()).Random();
            var sell5 = (new TradeOrderFrame()).Random();
            var sell6 = (new TradeOrderFrame()).Random();
            var sell7 = (new TradeOrderFrame()).Random();
            var sell8 = (new TradeOrderFrame()).Random();
            var sell9 = (new TradeOrderFrame()).Random();

            buy5.Position = OrderPosition.Buy;
            sell5.Position = OrderPosition.Sell;
            sell6.Position = OrderPosition.Sell;
            sell7.Position = OrderPosition.Sell;
            sell8.Position = OrderPosition.Sell;
            sell9.Position = OrderPosition.Sell;

            sell5.ExecutedPrice = new Price(sell1.ExecutedPrice.Value.Value * 1.40m, sell1.ExecutedPrice.Value.Currency);
            sell6.ExecutedPrice = new Price(sell5.ExecutedPrice.Value.Value * 1.01m, sell1.ExecutedPrice.Value.Currency);
            sell7.ExecutedPrice = new Price(sell5.ExecutedPrice.Value.Value * 1.01m, sell1.ExecutedPrice.Value.Currency);
            sell8.ExecutedPrice = new Price(sell5.ExecutedPrice.Value.Value * 0.99m, sell1.ExecutedPrice.Value.Currency);
            sell9.ExecutedPrice = new Price(sell5.ExecutedPrice.Value.Value, sell1.ExecutedPrice.Value.Currency);
            buy5.ExecutedPrice = new Price(sell5.ExecutedPrice.Value.Value * 1.01m, sell2.ExecutedPrice.Value.Currency);

            var tradeList = new List<TradeOrderFrame> { buy1, sell1, buy2, sell2, buy3, buy4, buy5, sell5, sell6, sell7, sell8, sell9 };

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
