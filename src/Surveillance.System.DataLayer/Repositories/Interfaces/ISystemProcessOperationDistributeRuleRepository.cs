using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.Systems.DataLayer.Processes.Interfaces;

namespace Surveillance.Systems.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationDistributeRuleRepository
    {
        Task Create(ISystemProcessOperationDistributeRule entity);
        Task Update(ISystemProcessOperationDistributeRule entity);
        Task<IReadOnlyCollection<ISystemProcessOperationDistributeRule>> GetDashboard();
    }
}