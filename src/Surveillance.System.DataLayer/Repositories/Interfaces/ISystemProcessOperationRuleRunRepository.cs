namespace Surveillance.Auditing.DataLayer.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public interface ISystemProcessOperationRuleRunRepository
    {
        Task Create(ISystemProcessOperationRuleRun entity);

        Task<IReadOnlyCollection<ISystemProcessOperationRuleRun>> Get(
            IReadOnlyCollection<string> systemProcessOperationIds);

        Task<IReadOnlyCollection<ISystemProcessOperationRuleRun>> GetDashboard();

        Task Update(ISystemProcessOperationRuleRun entity);
    }
}