using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services.Interfaces
{
    public interface IAthenaService
    {
        Task<string> StartQueryExecutionAsync(string database, string queryString, string outputLocation);

        Task PoolQueryExecutionAsync(string queryExecutionId);
    }
}
