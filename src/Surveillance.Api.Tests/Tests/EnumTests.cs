namespace Surveillance.Api.Tests.Tests
{
    using System;
    using System.Linq;

    using Domain.Core.Trading.Orders;

    using NUnit.Framework;

    using RedDeer.Surveillance.Api.Client.Enums;

    using OrderStatus = RedDeer.Surveillance.Api.Client.Enums.OrderStatus;

    [TestFixture]
    public class EnumTests
    {
        [Test]
        public void OrderDirection_Matches()
        {
            this.TestEnumMatches<OrderDirection, OrderDirections>();
        }

        [Test]
        public void OrderStatus_Matches()
        {
            this.TestEnumMatches<OrderStatus, Domain.Core.Trading.Orders.OrderStatus>();
        }

        [Test]
        public void OrderType_Matches()
        {
            this.TestEnumMatches<OrderType, OrderTypes>();
        }

        private void TestEnumMatches<A, B>()
        {
            var valuesA = Enum.GetValues(typeof(A)).Cast<A>().ToList();
            var valuesB = Enum.GetValues(typeof(B)).Cast<B>().ToList();

            Assert.That(valuesA.Count, Is.EqualTo(valuesB.Count));

            for (var i = 0; i < valuesA.Count; i++)
            {
                var a = valuesA[i];
                var b = valuesB[i];
                Assert.That(Convert.ToInt32(a), Is.EqualTo(Convert.ToInt32(b)));
                Assert.That(a.ToString().ToLower(), Is.EqualTo(b.ToString().ToLower()));
            }
        }
    }
}