using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationDistributeRuleRepository
    {
        void Create(ISystemProcessOperationDistributeRule entity);
    }
}