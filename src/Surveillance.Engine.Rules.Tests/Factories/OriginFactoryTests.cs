using System;
using NUnit.Framework;

namespace Surveillance.Tests.Factories
{
    [TestFixture]
    public class OriginFactoryTests
    {
        [Test]
        public void Origin_Returns_ExpectedResult()
        {
            var factory = new OriginFactory();

            var result = factory.Origin();

            Assert.AreEqual(result, $"{Environment.MachineName}:surveillance-service");
        }
    }
}
