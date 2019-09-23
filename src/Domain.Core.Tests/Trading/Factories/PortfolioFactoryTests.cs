namespace Domain.Core.Tests.Trading.Factories
{
    using Domain.Core.Trading.Factories;

    using NUnit.Framework;

    [TestFixture]
    public class PortfolioFactoryTests
    {
        [Test]
        public void Build_ReturnsDifferentPortfolio_PerInvokation()
        {
            var factory = new PortfolioFactory();

            var portfolioOne = factory.Build();
            var portfolioTwo = factory.Build();

            Assert.IsNotNull(portfolioOne);
            Assert.IsNotNull(portfolioTwo);
            Assert.AreNotEqual(portfolioOne, portfolioTwo);
        }
    }
}