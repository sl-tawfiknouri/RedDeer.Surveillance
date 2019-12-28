using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services.Interfaces
{
    public interface IAthenaService
    {
        Task<string> StartQueryExecutionAsync(string database, string queryString, string outputLocation);

        Task PoolQueryExecutionAsync(string queryExecutionId);

        Task BatchPoolQueryExecutionAsync(List<string> queryExecutionId, int delayMs = 5000);
    }
}
