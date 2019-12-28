using RedDeer.Etl.SqlSriptExecutor.Services.Models;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services.Interfaces
{
    public interface ISqlSriptExecutorService
    {
        Task<bool> ExecuteAsync(SqlSriptExecutorRequest request);
    }
}
