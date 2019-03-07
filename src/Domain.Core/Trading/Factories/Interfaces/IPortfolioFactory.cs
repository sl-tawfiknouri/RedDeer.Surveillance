using Domain.Core.Trading.Interfaces;

namespace Domain.Core.Trading.Factories.Interfaces
{
    public interface IPortfolioFactory
    {
        IPortfolio Build();
    }
}