using System;
using NUnit.Framework;
using Surveillance.Engine.Rules.Factories;

namespace Surveillance.Engine.Rules.Tests.Factories
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
