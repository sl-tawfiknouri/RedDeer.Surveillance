using NUnit.Framework;
using Surveillance.Universe;

namespace Surveillance.Tests.Universe
{
    [TestFixture]
    public class UniverseBuilderTests
    {
        [Test]
        public void Summon_DoesNot_ReturnNull()
        {
            var builder = new UniverseBuilder();

            var result = builder.Summon(null);

            Assert.IsNotNull(result);
        }
    }
}
