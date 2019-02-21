﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;

namespace Surveillance.Auditing.DataLayer.Repositories.Interfaces
{
    public interface ISystemProcessOperationRuleRunRepository
    {
        Task Create(ISystemProcessOperationRuleRun entity);
        Task Update(ISystemProcessOperationRuleRun entity);
        Task<IReadOnlyCollection<ISystemProcessOperationRuleRun>> GetDashboard();
        Task<IReadOnlyCollection<ISystemProcessOperationRuleRun>> Get(IReadOnlyCollection<string> systemProcessOperationIds);
    }
}