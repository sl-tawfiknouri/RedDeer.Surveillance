using Domain.Equity.Trading;
using FakeItEasy;
using NUnit.Framework;
using TestHarness.Engine.OrderGenerator.Strategies;

namespace TestHarness.Tests.Engine.OrderGenerator.Strategies
{
    [TestFixture]
    public class ProbabilisticTradeStrategyTests
    {
        private ITradeOrderStream _tradeOrderStream;

        [SetUp]
        public void Setup()
        {
            _tradeOrderStream = A.Fake<ITradeOrderStream>();
        }

        [Test]
        public void ExecuteTradeStrategy_NullTick_DoesNotThrow()
        {
            var tradeStrategy = new ProbabilisticTradeStrategy();

            Assert.DoesNotThrow(() => tradeStrategy.ExecuteTradeStrategy(null, _tradeOrderStream));
        }
    }
}
