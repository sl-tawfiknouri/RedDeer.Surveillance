namespace Domain.Core.Trading.Factories.Interfaces
{
    using Domain.Core.Trading.Interfaces;

    public interface IPortfolioFactory
    {
        IPortfolio Build();
    }
}