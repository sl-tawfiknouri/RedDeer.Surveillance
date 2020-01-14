using RedDeer.Etl.SqlSriptExecutor.Services.Models;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services.Interfaces
{
    public interface IFunctionService
    {
        Task<bool> ExecuteAsync(FunctionRequest request);
    }
}
