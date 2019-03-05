using System.Collections.Generic;

namespace Domain.Core.Trading.Factories
{
    public class PortfolioFactory : IPortfolioFactory
    {
        public Portfolio Build()
        {
            return new Portfolio(
                new Holdings(
                    new List<Holding>()),
                new OrderLedger());
        }
    }
}
