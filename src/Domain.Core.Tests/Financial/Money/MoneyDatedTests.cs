namespace Domain.Core.Tests.Financial.Money
{
    using System;
    using Domain.Core.Financial.Money;
    using NUnit.Framework;

    [TestFixture]
    public class MoneyDatedTests
    {
        [Test]
        public void Ctor_NullCurrency_DoesNotThrow()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new MoneyDated(100, null, DateTime.UtcNow));
        }

        [Test]
        public void Ctor_AssignsValues_Correctly()
        {
            var date = DateTime.UtcNow;
            var money = new MoneyDated(100, "GBP", date);

            Assert.AreEqual(100, money.Amount.Value);
            Assert.AreEqual("GBP", money.Amount.Currency.Code);
            Assert.AreEqual(date, money.Date);
        }
    }
}
