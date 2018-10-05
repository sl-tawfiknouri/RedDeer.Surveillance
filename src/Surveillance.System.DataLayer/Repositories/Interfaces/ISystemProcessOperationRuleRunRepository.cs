using System.Threading.Tasks;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationRuleRunRepository
    {
        Task Create(ISystemProcessOperationRuleRun entity);
        Task Update(ISystemProcessOperationRuleRun entity);
    }
}