using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RedDeer.Surveillance.Api.Client;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests.Tests
{
    public abstract class BaseTest
    {
        protected Infrastructure.DbContext _dbContext { get => Dependencies.DbContext; }
        protected ApiClient _apiClient { get => Dependencies.ApiClient; }

        [SetUp]
        public async Task SetupBeforeEachTest()
        {
            RemoveAll(_dbContext.DbRuleBreaches);
            RemoveAll(_dbContext.DbRuleBreachOrders);
            RemoveAll(_dbContext.DbOrders);
            RemoveAll(_dbContext.DbRuleRuns);
            RemoveAll(_dbContext.DbProcessOperations);
            RemoveAll(_dbContext.DbOrderAllocations);
            RemoveAll(_dbContext.DbMarkets);
            RemoveAll(_dbContext.DbBrokers);
            await _dbContext.SaveChangesAsync();
        }

        private void RemoveAll<T>(DbSet<T> dbSet) where T : class
        {
            foreach (var item in dbSet)
            {
                dbSet.Remove(item);
            }
        }
    }
}
