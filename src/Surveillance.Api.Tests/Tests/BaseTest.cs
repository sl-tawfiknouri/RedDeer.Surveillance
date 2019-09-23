namespace Surveillance.Api.Tests.Tests
{
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using NUnit.Framework;

    using RedDeer.Surveillance.Api.Client;

    using DbContext = Surveillance.Api.Tests.Infrastructure.DbContext;

    public abstract class BaseTest
    {
        protected ApiClient _apiClient => Dependencies.ApiClient;

        protected DbContext _dbContext => Dependencies.DbContext;

        [SetUp]
        public async Task SetupBeforeEachTest()
        {
            this.RemoveAll(this._dbContext.DbRuleBreaches);
            this.RemoveAll(this._dbContext.DbRuleBreachOrders);
            this.RemoveAll(this._dbContext.DbOrders);
            this.RemoveAll(this._dbContext.DbRuleRuns);
            this.RemoveAll(this._dbContext.DbProcessOperations);
            this.RemoveAll(this._dbContext.DbOrderAllocations);
            this.RemoveAll(this._dbContext.DbMarkets);
            this.RemoveAll(this._dbContext.DbBrokers);
            await this._dbContext.SaveChangesAsync();
        }

        private void RemoveAll<T>(DbSet<T> dbSet)
            where T : class
        {
            foreach (var item in dbSet) dbSet.Remove(item);
        }
    }
}