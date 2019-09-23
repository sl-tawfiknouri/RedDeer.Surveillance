namespace Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory
{
    public interface IGraphQlDbContextFactory
    {
        IGraphQlDbContext Build();
    }
}