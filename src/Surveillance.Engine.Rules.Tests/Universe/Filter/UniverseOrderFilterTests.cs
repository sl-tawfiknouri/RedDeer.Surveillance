using System;
using Domain.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Engine.Rules.Tests.Helpers;
using Surveillance.Engine.Rules.Universe;
using Surveillance.Engine.Rules.Universe.Filter;

namespace Surveillance.Engine.Rules.Tests.Universe.Filter
{
    [TestFixture]
    public class UniverseOrderFilterTests
    {
        private ILogger<UniverseEquityOrderFilter> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<UniverseEquityOrderFilter>>();
        }

        [TestCase("d")]
        [TestCase("D")]
        [TestCase("Deerbc")]
        [TestCase("Olekg")]
        [TestCase("")]
        [TestCase(null)]
        public void Filter_FiltersOutCredit_Cfi(string nonEquityCfi)
        {
            var orderFilter = new UniverseEquityOrderFilter(_logger);
            var order = ((Order) null).Random();
            var universeEvent = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, order);
            order.Instrument.Cfi = nonEquityCfi;

            var filteredEvent = orderFilter.Filter(universeEvent);

            Assert.IsNull(filteredEvent);
        }

        [TestCase("e")]
        [TestCase("E")]
        [TestCase("entspb")]
        [TestCase("ENTSPB")]
        [TestCase("eNt")]
        public void Filter_DoesNotFiltersOutEquity_Cfi(string equityCfi)
        {
            var orderFilter = new UniverseEquityOrderFilter(_logger);
            var order = ((Order)null).Random();
            var universeEvent = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, order);
            order.Instrument.Cfi = equityCfi;

            var filteredEvent = orderFilter.Filter(universeEvent);

            Assert.AreEqual(universeEvent, filteredEvent);
        }

        [Test]
        public void Filter_NonOrderEvent_ReturnsEvent()
        {
            var orderFilter = new UniverseEquityOrderFilter(_logger);
            var universeEvent = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, "not-an-order");

            var filteredEvent = orderFilter.Filter(universeEvent);

            Assert.AreEqual(universeEvent, filteredEvent);
        }
    }
}
