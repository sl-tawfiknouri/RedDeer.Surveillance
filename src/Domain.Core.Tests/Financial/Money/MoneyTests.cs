namespace Domain.Core.Tests.Financial.Money
{
    using System;

    using Domain.Core.Financial.Money;
    using NUnit.Framework;

    [TestFixture]
    public class MoneyTests
    {
        [Test]
        public void Ctor_BuildsMoneyWithNull_Currency()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new Money(0, null));
        }

        [Test]
        public void Ctor_BuildsMoneyWithOk_Currency()
        {
            var currency = new Currency("GBP");

            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new Money(0, currency));
        }

        [Test]
        public void Ctor_AssignsVariables_Correctly()
        {
            var currency = new Currency("GBP");

            var money = new Money(100, currency);

            Assert.AreEqual(100, money.Value);
            Assert.AreEqual(money.Currency, currency);
        }

        [Test]
        public void Plus_AddsTwoMoneyAmounts_Correctly()
        {
            var moneyA = new Money(100, "GBP");
            var moneyB = new Money(50, "GBP");

            var result = moneyA + moneyB;

            Assert.AreEqual("GBP", result.Currency.Code);
            Assert.AreEqual(150, result.Value);
        }

        [Test]
        public void Plus_AddsTwoMoneyAmountsButDifferentCurrency_ThrowsException()
        {
            var moneyA = new Money(100, "GBP");
            var moneyB = new Money(50, "XOX");

            Assert.Throws<ArgumentException>(() =>
                {
                    var _ = moneyA + moneyB;
                });
        }

        [Test]
        public void Plus_SubtractTwoMoneyAmounts_Correctly()
        {
            var moneyA = new Money(100, "GBP");
            var moneyB = new Money(50, "GBP");

            var result = moneyA - moneyB;

            Assert.AreEqual("GBP", result.Currency.Code);
            Assert.AreEqual(50, result.Value);
        }

        [Test]
        public void Plus_SubtractTwoMoneyAmountsButDifferentCurrency_ThrowsException()
        {
            var moneyA = new Money(100, "GBP");
            var moneyB = new Money(50, "XOX");

            Assert.Throws<ArgumentException>(() =>
                {
                    var _ = moneyA - moneyB;
                });
        }

        [Test]
        public void ToString_ReturnsExpected_Str()
        {
            var money = new Money(100, "USD");

            var result = money.ToString();

            Assert.AreEqual("(USD) 100", result);
        }
    }
}
