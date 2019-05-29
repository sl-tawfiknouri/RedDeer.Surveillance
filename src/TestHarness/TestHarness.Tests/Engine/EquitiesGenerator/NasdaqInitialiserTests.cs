using NUnit.Framework;
using TestHarness.Engine.EquitiesGenerator;

namespace TestHarness.Tests.Engine.EquitiesGenerator
{
    [TestFixture]
    public class NasdaqInitialiserTests
    {
        [Test]
        public void InitialTick_ContainsExpectedNumberOfSecurities()
        {
            var initialiser = new NasdaqInitialiser();

            var tick = initialiser.InitialFrame();

            Assert.AreEqual(tick.Exchange.Name, "NASDAQ");
            Assert.AreEqual(tick.Securities.Count, 3109);
        }
    }
}
