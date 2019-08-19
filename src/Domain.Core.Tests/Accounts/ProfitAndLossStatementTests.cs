using Domain.Core.Accounts;
using Domain.Core.Financial.Money;
using NUnit.Framework;

namespace Domain.Core.Tests.Accounts
{
    [TestFixture]
    public class ProfitAndLossStatementTests
    {
        [TestCase(100, 100, 0)]
        [TestCase(-100, 100, -200)]
        [TestCase(100, -100, 200)]
        [TestCase(100, 0, 100)]
        [TestCase(0, 100, -100)]
        [TestCase(10000, 10000, 0)]
        [TestCase(200, 100, 100)]
        [TestCase(100, 200, -100)]
        public void Profits_Returns_Revenue_Minus_Costs(decimal revenue, decimal costs, decimal expectedResult)
        {
            var currency = new Currency("GBP");
            var revenueMoney = new Money(revenue, currency);
            var costMoney = new Money(costs, currency);
            var statement = new ProfitAndLossStatement(currency, revenueMoney, costMoney);

            var profits = statement.Profits();

            Assert.IsNotNull(profits);
            Assert.AreEqual(profits.Currency, currency);
            Assert.AreEqual(profits.Value, expectedResult);
        }

        [TestCase(100, 100, 0)]
        [TestCase(-100, 100, -2)]
        [TestCase(100, -100, -2)]
        [TestCase(100, 0, null)]
        [TestCase(0, 100, null)]
        [TestCase(10000, 10000, 0)]
        [TestCase(200, 100, 1)]
        [TestCase(100, 200, -0.5)]
        public void Profits_Returns_Revenue_Minus_Costs_Percentages(decimal revenue, decimal costs, decimal? expectedResult)
        {
            var currency = new Currency("GBP");
            var revenueMoney = new Money(revenue, currency);
            var costMoney = new Money(costs, currency);
            var statement = new ProfitAndLossStatement(currency, revenueMoney, costMoney);

            var profits = statement.PercentageProfits();

            Assert.AreEqual(profits.HasValue, expectedResult.HasValue);

            if (expectedResult.HasValue)
                Assert.AreEqual(profits.Value, expectedResult);
        }

        [Test]
        public void Profits_Empty_Returns_Statement_With_Expected_Null_Object_Pattern_Values()
        {
            var empty = ProfitAndLossStatement.Empty();

            Assert.IsNotNull(empty);
            Assert.AreEqual(empty.Denomination.Code, "GBP");
            Assert.AreEqual(empty.Costs.Value, 0);
            Assert.AreEqual(empty.Costs.Currency.Code, "GBP");
            Assert.AreEqual(empty.Revenue.Value, 0);
            Assert.AreEqual(empty.Revenue.Currency.Code, "GBP");
        }
    }
}
