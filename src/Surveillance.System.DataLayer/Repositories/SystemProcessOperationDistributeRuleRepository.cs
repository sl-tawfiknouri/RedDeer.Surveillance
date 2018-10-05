﻿using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessOperationDistributeRuleRepository : ISystemProcessOperationDistributeRuleRepository
    {
        public void Create(ISystemProcessOperationDistributeRule entity)
        {
            if (entity == null)
            {
                return;
            }

            // TODO handle saving to Aurora
        }
    }
}
