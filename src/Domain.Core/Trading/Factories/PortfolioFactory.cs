using System.Collections.Generic;

namespace Domain.Core.Trading.Factories
{
    public class PortfolioFactory : IPortfolioFactory
    {
        public IPortfolio Build()
        {
            return new Portfolio(
                new Holdings(
                    new List<Holding>()),
                new OrderLedger());
        }
    }
}
