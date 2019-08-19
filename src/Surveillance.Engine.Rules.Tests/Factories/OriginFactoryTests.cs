namespace Surveillance.Engine.Rules.Tests.Factories
{
    using System;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.Factories;

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