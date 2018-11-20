using System;
using NUnit.Framework;
using Surveillance.Factories;

namespace Surveillance.Tests.Factories
{
    [TestFixture]
    public class LayeringFactoryTests
    {
        [Test]
        public void Constructor_ConsidersNullLogger_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRuleRuleFactory(null));
        }
    }
}
