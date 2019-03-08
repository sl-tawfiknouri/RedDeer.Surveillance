using System.Collections.Generic;
using Domain.Core.Trading.Factories.Interfaces;
using Domain.Core.Trading.Interfaces;

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
