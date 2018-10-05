using System.Threading.Tasks;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationDistributeRuleRepository
    {
        Task Create(ISystemProcessOperationDistributeRule entity);
        Task Update(ISystemProcessOperationDistributeRule entity);
    }
}