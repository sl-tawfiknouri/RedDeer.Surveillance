using NUnit.Framework;
using TestHarness.EquitiesGenerator;

namespace TestHarness.Tests.EquitiesGenerator
{
    [TestFixture]
    public class NasdaqInitialiserTests
    {
        [Test]
        public void InitialTick_ContainsExpectedNumberOfSecurities()
        {
            var initialiser = new NasdaqInitialiser();

            var tick = initialiser.InitialTick();

            Assert.AreEqual(tick.Exchange.Name, "NASDAQ");
            Assert.AreEqual(tick.Securities.Count, 3109);
        }
    }
}
