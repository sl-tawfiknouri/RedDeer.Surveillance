using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services.Interfaces
{
    public interface IAthenaClientService
    {
        Task<string> StartQueryExecutionAsync(string database, string queryString, string outputLocation);

        Task PoolQueryExecutionAsync(string queryExecutionId, int delayMs = 5000);

        Task BatchPoolQueryExecutionAsync(List<string> queryExecutionId, int delayMs = 5000);
    }
}
