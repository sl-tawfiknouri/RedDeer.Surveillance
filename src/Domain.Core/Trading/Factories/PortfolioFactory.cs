namespace Domain.Core.Trading.Factories
{
    using Domain.Core.Trading.Factories.Interfaces;
    using Domain.Core.Trading.Interfaces;

    public class PortfolioFactory : IPortfolioFactory
    {
        public IPortfolio Build()
        {
            return new Portfolio(new OrderLedger());
        }
    }
}