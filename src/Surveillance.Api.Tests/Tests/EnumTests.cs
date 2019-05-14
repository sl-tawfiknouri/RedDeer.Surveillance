using NUnit.Framework;
using Surveillance.Api.Client.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surveillance.Api.Tests.Tests
{
    public class EnumTests
    {
        [Test]
        public void OrderType_Matches()
        {
            TestEnumMatches<OrderType, Domain.Core.Trading.Orders.OrderTypes>();
        }

        [Test]
        public void OrderDirection_Matches()
        {
            TestEnumMatches<OrderDirection, Domain.Core.Trading.Orders.OrderDirections>();
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
                Assert.That(a.ToString(), Is.EqualTo(b.ToString()));
            }
        }
    }
}
