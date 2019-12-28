using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services.Interfaces
{
    public interface IS3ClientService
    {
        Task<string> ReadAllText(string uri);
    }
}
