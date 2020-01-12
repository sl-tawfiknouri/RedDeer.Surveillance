using RedDeer.Etl.SqlSriptExecutor.Services.Models;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services.Interfaces
{
    public interface IFilePreProcessorService
    {
        Task<bool> PreProcessAsync(FilePreProcessorData data);
    }
}
